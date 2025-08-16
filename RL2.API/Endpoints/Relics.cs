using MonoMod.RuntimeDetour;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using Rewired.Utils.Libraries.TinyJson;

namespace RL2.API;

/// <summary>
///	Provides endpoints for Relic related APIs
/// </summary>
public static class Relics
{
	internal static IDetour[] Hooks = [
		SaveData.Hook,
		LoadData.Hook,
		ModifyData.Hook,
		ExtendTypeArray.Hook,
		ApplyEffect.Hook,
		StopEffect.Hook,
	];

	internal static Dictionary<RelicType, RelicData> ModdedStore = [];

	internal static Dictionary<string, RelicType> SavedTypes = [];

	internal static Dictionary<RelicType, string> SavedNames = [];

	internal static Dictionary<string, bool> SavedFoundState = [];

	internal static int LastType = (int)RelicType.DeathMark;

	internal static bool FirstLoad = true;

	/// <summary>
	/// Registers the custom Relic
	/// </summary>
	/// <param name="data">Data of the custom Relic</param>
	/// <param name="icon">Small icon - displayed in the HUD</param>
	/// <param name="iconBig">Big icon - displayed when picking up the Relic</param>
	public static RelicType Register(RelicData data, Texture2D? icon = null, Texture2D? iconBig = null) {
		while (IconLibrary.Instance == null) { }

		string modName = RL2API.AssemblyToMod[Assembly.GetCallingAssembly()].Manifest.Name;
		string name = modName + "/" + data.Name;

		RelicType type = SavedTypes.TryGetValue(name, out RelicType value) ? value : (RelicType)(++LastType);
		SavedTypes[name] = type;
		SavedNames[type] = name;

		ModdedStore[type] = data;

		// Add Relic icons
		Sprite relicSprite = IconLibrary.Instance.m_defaultSprite;
		if (icon != null) {
			relicSprite = Sprite.Create(icon, new Rect(0, 0, icon.width / 2, icon.height / 2), new Vector2(.5f, .5f));
		}
		IconLibrary.Instance.m_relicIconLibrary.Add(type, relicSprite);

		Sprite relicSpriteBig = IconLibrary.Instance.m_defaultSprite;
		if (iconBig != null) {
			relicSpriteBig = Sprite.Create(iconBig, new Rect(0, 0, iconBig.width, iconBig.height), new Vector2(.5f, .5f));
		}
		IconLibrary.Instance.m_relicLargeIconLibrary.Add(type, relicSpriteBig);

		// Manage seen state
		RelicObj obj = new RelicObj(type) {
			WasSeen = SavedFoundState.TryGetValue(name, out bool seen) ? seen : false
		};
		SaveManager.PlayerSaveData.RelicObjTable[type] = obj;
		SavedFoundState[name] = obj.WasSeen;

		RL2API.Log($"Saved {data.Name} as {type}");
		return type;
	}
	
	/// <summary>
	/// Ran when Relic data is being loaded. <br></br>
	/// Current save profile canm be accessed via <see cref="SaveManager.CurrentProfile"/>
	/// </summary>
	public static class SaveData {
		/// <inheritdoc cref="SaveData"/>
		public delegate void Definition();

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static Hook Hook = new Hook(
			typeof(SaveManager).GetMethod(nameof(SaveManager.LoadAllGameData), BindingFlags.Public | BindingFlags.Static),
			Method,
			new HookConfig() {
				ID = "RL2.API::Relics.SaveData",
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
			
		}
	}

	/// <summary>
	/// Ran when Relic data is being loaded.
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
				ID = "RL2.API::Relics.LoadData",
				ManualApply = true,
			}
		);

		internal static LOAD_RESULT Method(Func<int, bool, LOAD_RESULT> orig, int currentProfile, bool loadAccountData) {
			LOAD_RESULT result = orig(currentProfile, loadAccountData);
			SavedFoundState.Clear();
			
			string directory = Path.Combine(SaveManager.GetConfigPath(), "RL2.API");
			if (Directory.Exists(directory)) Directory.CreateDirectory(directory);

			directory = Path.Combine(directory, "Relics");
			if (Directory.Exists(directory)) Directory.CreateDirectory(directory);

			string savedFoundState = Path.Combine(
				SaveManager.GetSaveDirectoryPath(SaveManager.CurrentProfile, false), "RL2.API", "Relics", "FoundState.json"
			);

			if (File.Exists(savedFoundState)) {
				SavedFoundState = JsonParser.FromJson<Dictionary<string, bool>>(File.ReadAllText(savedFoundState));
			}

			if (FirstLoad) LoadSavedModdedTypes();

			return result;
		}

		internal static void LoadSavedModdedTypes() {
			string savedTypes = Path.Combine(
				SaveManager.GetSaveDirectoryPath(SaveManager.CurrentProfile, false), "RL2.API", "Relics", "Types.json"
			);

			if (File.Exists(savedTypes)) {
				SavedTypes = JsonParser.FromJson<Dictionary<string, RelicType>>(File.ReadAllText(savedTypes));
				var sorted = SavedTypes.Values.ToList();
				sorted.Sort();
				if (sorted.Count > 0)
					LastType = (int)sorted.Last();
			}

			FirstLoad = false;
			Event?.Invoke();
		}
	}

	/// <summary> 
	///	Allows for modifying Relic data
	/// </summary>
	public static class ModifyData
	{
		/// <inheritdoc cref="ModifyData"/>
		/// <param name="type"> Relic type </param>
		/// <param name="data"> Relic data </param>
		public delegate void Definition(RelicType type, RelicData data);

		/// <inheritdoc cref="Definition" />
		public static event Definition? Event;

		internal static Hook Hook = new Hook(
			typeof(RelicLibrary).GetMethod("GetRelicData", BindingFlags.Public | BindingFlags.Static),
			Method,
			new HookConfig() {
				ID = "RL2.API::Relics.ModifyData",
				ManualApply = true,
			}
		);

		internal static RelicData Method(Func<RelicType, RelicData> orig, RelicType type) {
			RelicData data = orig(type);
			if (data == null) {
				data = Relics.ModdedStore[type];
			}
			Event?.Invoke(type, data);
			return data;
		}
	}

	/// <summary>
	/// Allows extending the <see cref="RelicType_RL.TypeArray"/>
	/// </summary>
	public static class ExtendTypeArray
	{
		/// <inheritdoc cref="ExtendTypeArray"/>
		/// <param name="list"></param>
		public delegate void Definition(ref List<RelicType> list);

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static Hook Hook = new Hook(
			typeof(RelicType_RL).GetProperty("TypeArray", BindingFlags.Public | BindingFlags.Static).GetGetMethod(),
			Method,
			new HookConfig() {
				ID = "RL2.API::Relics.ExtendTypeArray",
				ManualApply = true,
			}
		);

		internal static RelicType[] Method(Func<RelicType[]> orig) {
			List<RelicType> original = orig().ToList();
			original.AddRange(ModdedStore.Keys.Cast<RelicType>());
			Event?.Invoke(ref original);
			return original.ToArray();
		}
	}

	/// <summary>
	/// Used to disable additional Relic functionality when the Relic is taken away
	/// </summary>
	public static class ApplyEffect
	{
		/// <inheritdoc cref="Definition"/>
		/// <param name="type">Activated Relic</param>
		public delegate void Definition(RelicType type);

		/// <inheritdoc cref="Definition" />
		public static event Definition? Event;

		internal static Hook Hook = new Hook(
			typeof(RelicObj).GetMethod("ApplyRelic", BindingFlags.Public | BindingFlags.Instance),
			Method,
			new HookConfig() {
				ID = "RL2.API::Relics.ApplyEffect",
				ManualApply = true,
			}
		);

		internal static void Method(Action<RelicObj, int> orig, RelicObj self, int levelChange) {
			orig(self, levelChange);
			if (SavedNames.TryGetValue(self.RelicType, out string name)) {
				SavedFoundState[name] = true;
			}
			Event?.Invoke(self.RelicType);
		}
	}

	/// <summary>
	/// Allows running code after Relic level change
	/// </summary>
	public static class StopEffect
	{
		/// <inheritdoc cref="Definition"/>
		/// <param name="type">Activated Relic</param>
		public delegate void Definition(RelicType type);

		/// <inheritdoc cref="Definition" />
		public static event Definition? Event;

		internal static Hook Hook = new Hook(
			typeof(RelicObj).GetMethod("StopRelic", BindingFlags.Public | BindingFlags.Instance),
			Method,
			new HookConfig() {
				ID = "RL2.API::Relics.StopEffect",
				ManualApply = true,
			}
		);

		internal static void Method(Action<RelicObj, int> orig, RelicObj self, int levelChange) {
			orig(self, levelChange);
			Event?.Invoke(self.RelicType);
		}
	}
}
