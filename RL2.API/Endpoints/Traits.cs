using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Rewired.Utils.Libraries.TinyJson;
using UnityEngine;

namespace RL2.API;

/// <summary>
/// Provides endpoints for Trait related APIs
/// </summary>
public static class Traits
{
	internal static IDetour[] Hooks = [
		SaveData.Hook,
		LoadData.Hook,
		LoadContent.Hook,
		ApplyEffect.Hook,
		ModifyData.Hook,
		ModifyTraitObj.Hook,
		ExtendTypeArray.Hook,
	];

	internal static Dictionary<TraitType, BaseTrait> ModdedStore = [];

	internal static Dictionary<string, TraitType> NameToType = [];

	internal static Dictionary<TraitType, string> TypeToName = [];

	internal static Dictionary<string, TraitSeenState> NameToFoundState = [];

	internal static int LastType = (int)TraitType.Antique;

	internal static bool FirstLoad = true;

	/// <summary>
	/// Extra data for Traits, optional
	/// </summary>
	public struct ExtraData
	{
		/// <summary>
		/// Which Trait this trait is incompatible with 
		/// </summary>
		public TraitType[] IncompatibleTraits;
		/// <summary>
		/// Post processing
		/// </summary>
		public MobilePostProcessOverrideController PostProcessOverride;
		/// <summary>
		/// Post processing mask
		/// </summary>
		public SpriteRenderer TraitMask;
	}

	/// <summary>
	/// Registers a new Trait
	/// </summary>
	/// <typeparam name="T">The Trait class</typeparam>
	/// <returns><see cref="TraitType"/> of the newly registered Trait</returns>
	public static TraitType Register<T>(TraitData data, ExtraData? extraData = null, Texture2D? icon = null) where T : BaseTrait {
		while (IconLibrary.Instance == null) { }

		string modName = RL2API.AssemblyToMod[Assembly.GetCallingAssembly()].Manifest.Name;
		string name = modName + "/" + data.Name;

		TraitType type = NameToType.TryGetValue(name, out TraitType savedType) ? savedType : (TraitType)(++LastType);
		NameToType[name] = type;
		TypeToName[type] = name;

		TraitSeenState seen = NameToFoundState.TryGetValue(name, out TraitSeenState savedState) ? savedState : TraitSeenState.SeenTwice;
		NameToFoundState[name] = seen;

		GameObject traitObject = new GameObject();
		UnityEngine.Object.DontDestroyOnLoad(traitObject);

		T trait = traitObject.AddComponent<T>();
		trait.enabled = false;
		trait.m_traitData = data;
		trait.m_incompatibleTraits = extraData?.IncompatibleTraits ?? [];
		trait.m_postProcessOverrideController = extraData?.PostProcessOverride;
		trait.m_traitMask = extraData?.TraitMask;

		ModdedStore[type] = trait;

		TraitManager.TraitSpawnOddsTable[type] = new Vector3Int(0, 0, (int)seen);
		SaveManager.PlayerSaveData.TraitSeenTable[type] = seen;
		
		Sprite traitIcon = IconLibrary.Instance.m_defaultSprite;
		if (icon != null) {
			traitIcon = Sprite.Create(icon, new Rect(0, 0, icon.width / 2, icon.height / 2), new Vector2(.5f, .5f));
		}
		IconLibrary.Instance.m_traitIconLibrary[type] = traitIcon;

		RL2API.Log($"Saved Trait {data.Name} as {type}");

		return type;
	}

	/// <summary>
	/// Retrieves the <see cref="TraitType"/> of a modded Trait
	/// </summary>
	/// <param name="TraitName">Name of the Trait in the format "Mod Name From Manifest/TraitName"</param>
	/// <returns><see cref="TraitType.None"/> if Trait was not found</returns>
	public static TraitType GetType(string TraitName) {
		TraitType type = TraitType.None;
		NameToType.TryGetValue(TraitName, out type);
		return type;
	}

	/// <summary>
	/// Allows modifying Trait data
	/// </summary>
	public static class ModifyData
	{
		/// <inheritdoc cref="ModifyData"/>
		/// <param name="type">Trait type</param>
		/// <param name="data">Trait data</param>
		public delegate void Definition(TraitType type, TraitData data);

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static Hook Hook = new Hook(
			typeof(TraitLibrary).GetMethod(nameof(TraitLibrary.GetTraitData), BindingFlags.Public | BindingFlags.Static),
			Method,
			new HookConfig() {
				ID = "RL2.API::Traits.ModifyData",
				ManualApply = true,
			}
		);

		internal static TraitData? Method(Func<TraitType, TraitData> orig, TraitType type) {
			TraitData? data = orig(type);
			if (data == null && type != TraitType.None) {
				ModdedStore.TryGetValue(type, out BaseTrait trait);
				data = trait?.m_traitData;
			}
			if (data != null)
				Event?.Invoke(type, data);

			return data;
		}
	}

	/// <summary>
	/// Allows modifying the <see cref="BaseTrait"/> object
	/// </summary>
	public static class ModifyTraitObj
	{
		/// <inheritdoc cref="ModifyTraitObj"/>
		/// <param name="trait">The retreived <see cref="BaseTrait"/> object</param>
		public delegate void Definition(BaseTrait trait);

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static Hook Hook = new Hook(
			typeof(TraitLibrary).GetMethod(nameof(TraitLibrary.GetTrait), BindingFlags.Public | BindingFlags.Static),
			Method,
			new HookConfig() {
				ID = "RL2.API::Traits.ModifyTraitObj",
				ManualApply = true,
			}
		);

		internal static BaseTrait? Method(Func<TraitType, BaseTrait> orig, TraitType type) {
			BaseTrait? trait = orig(type);
			if (trait == null)
				ModdedStore.TryGetValue(type, out trait);

			if (trait != null)
				Event?.Invoke(trait);

			return trait;
		}
	}

	/// <summary>
	/// Triggered when a Trait is applied
	/// </summary>
	public static class ApplyEffect
	{
		/// <inheritdoc cref="ApplyEffect"/>
		/// <param name="type"></param>
		public delegate void Definition(TraitType type);

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static Hook Hook = new Hook(
			typeof(TraitManager).GetMethod(nameof(TraitManager.InstantiateTrait), BindingFlags.NonPublic | BindingFlags.Instance),
			Method,
			new HookConfig() {
				ID = "RL2.API::Traits.ApplyEffect",
				ManualApply = true,
			}
		);

		internal static void Method(Action<TraitManager, TraitType, bool> orig, TraitManager self, TraitType type, bool isTraitOne) {
			orig(self, type, isTraitOne);
			Event?.Invoke(type);
		}
	}

	/// <summary>
	/// Allows extending the <see cref="TraitType_RL.TypeArray"/>
	/// </summary>
	public static class ExtendTypeArray
	{
		/// <inheritdoc cref="ExtendTypeArray"/>
		/// <param name="list"></param>
		public delegate void Definition(ref List<TraitType> list);

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static Hook Hook = new Hook(
			typeof(TraitType_RL).GetProperty("TypeArray", BindingFlags.Public | BindingFlags.Static).GetGetMethod(),
			Method,
			new HookConfig() {
				ID = "RL2.API::Traits.ExtendTypeArray",
				ManualApply = true,
			}
		);

		internal static TraitType[] Method(Func<TraitType[]> orig) {
			List<TraitType> original = orig().ToList();
			original.AddRange(ModdedStore.Keys);
			Event?.Invoke(ref original);
			return original.ToArray();
		}
	}

	/// <summary>
	/// Ran when Trait data is being saved. <br></br>
	/// Current save profile canm be accessed via <see cref="SaveManager.CurrentProfile"/>
	/// </summary>
	public static class SaveData
	{
		/// <inheritdoc cref="SaveData"/>
		public delegate void Definition();

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static Hook Hook = new Hook(
			typeof(SaveManager).GetMethod(nameof(SaveManager.LoadAllGameData), BindingFlags.Public | BindingFlags.Static),
			Method,
			new HookConfig() {
				ID = "RL2.API::Traits.SaveData",
				ManualApply = true,
			}
		);

		internal static LOAD_RESULT Method(Func<int, bool, LOAD_RESULT> orig, int currentProfile, bool loadAccountData) {
			SaveModdedData();
			Event?.Invoke();
			LOAD_RESULT result = orig(currentProfile, loadAccountData);
			return result;
		}

		internal static void SaveModdedData() {
			// Nothing has been loaded, so there is nothing to save
			if (FirstLoad) return;

			// Save Trait types
			string directory = Path.Combine(SaveFileSystem.PersistentDataPath, "Saves", "RL2.API");
			if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

			directory = Path.Combine(directory, "Traits");
			if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

			string saveTypes = Path.Combine(directory, "Types.json");
			File.WriteAllText(saveTypes, JsonWriter.ToJson(NameToType));

			// Save found state
			directory = Path.Combine(SaveManager.GetSaveDirectoryPath(SaveManager.CurrentProfile, false), "RL2.API");
			if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

			directory = Path.Combine(directory, "Traits");
			if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

			string savedFoundState = Path.Combine(directory, "FoundState.json");
			File.WriteAllText(savedFoundState, JsonWriter.ToJson(NameToFoundState));
		}
	}

	/// <summary>
	/// Ran when Trait data is being loaded for the first time.
	/// </summary>
	public static class LoadData
	{
		/// <inheritdoc cref="LoadData"/>
		public delegate void Definition();

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static Hook Hook = new Hook(
			typeof(SaveManager).GetMethod(nameof(SaveManager.LoadAllGameData), BindingFlags.Public | BindingFlags.Static),
			Method,
			new HookConfig() {
				ID = "RL2.API::Traits.LoadData",
				ManualApply = true,
			}
		);

		internal static LOAD_RESULT Method(Func<int, bool, LOAD_RESULT> orig, int currentProfile, bool loadAccountData) {
			LOAD_RESULT result = orig(currentProfile, loadAccountData);
			NameToFoundState.Clear();
			LoadFoundState();

			if (FirstLoad) LoadSavedTypes();
			else {
				foreach ((string name, TraitSeenState state) in NameToFoundState)
					SaveManager.PlayerSaveData.TraitSeenTable[NameToType[name]] = state;
			}

			Event?.Invoke();

			return result;
		}

		internal static void LoadSavedTypes() {
			string directory = Path.Combine(SaveFileSystem.PersistentDataPath, "Saves", "RL2.API");
			if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

			directory = Path.Combine(directory, "Traits");
			if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

			string savedTypes = Path.Combine(directory, "Types.json");
			if (File.Exists(savedTypes)) {
				NameToType = JsonParser.FromJson<Dictionary<string, TraitType>>(File.ReadAllText(savedTypes));
				var sorted = NameToType.Values.ToList();
				sorted.Sort();
				if (sorted.Count > 0)
					LastType = (int)sorted.Last();
			}
		}

		internal static void LoadFoundState() {
			string directory = Path.Combine(SaveManager.GetSaveDirectoryPath(SaveManager.CurrentProfile, false), "RL2.API");
			if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

			directory = Path.Combine(directory, "Traits");
			if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

			string savedFoundState = Path.Combine(directory, "FoundState.json");
			if (File.Exists(savedFoundState)) {
				NameToFoundState = JsonParser.FromJson<Dictionary<string, TraitSeenState>>(File.ReadAllText(savedFoundState));
			}
		}
	}

	/// <summary>
	/// Use to register Traits with <see cref="Traits.Register{T}(TraitData, ExtraData?, Texture2D?)"/>
	/// </summary>
	/// <remarks>This is ran only once for the entire game session</remarks>
	public static class LoadContent
	{
		/// <inheritdoc cref="LoadContent"/>
		public delegate void Definition();

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static Hook Hook = new Hook(
			typeof(SaveManager).GetMethod(nameof(SaveManager.LoadAllGameData), BindingFlags.Public | BindingFlags.Static),
			Method,
			new HookConfig() {
				ID = "RL2.API::Traits.LoadContent",
				ManualApply = true,
				After = ["RL2.API::Traits.LoadData"]
			}
		);

		internal static LOAD_RESULT Method(Func<int, bool, LOAD_RESULT> orig, int currentProfile, bool loadAccountData) {
			LOAD_RESULT result = orig(currentProfile, loadAccountData);

			if (FirstLoad) Event?.Invoke();
			FirstLoad = false;

			return result;
		}
	}
}