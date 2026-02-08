using Reflect = System.Reflection;
using Collections = System.Collections.Generic;
using MM = MonoMod.RuntimeDetour;
using Cil = MonoMod.Cil;
using IL = Mono.Cecil.Cil;
using UnityE = UnityEngine;
using TinyJson = Rewired.Utils.Libraries.TinyJson;
using IO = System.IO;
using Loader = RL2.ModLoader;

namespace RL2.API;

/// <summary> </summary>
public static class Burdens {
	internal static MM.IDetour[] Hooks = [
		SilenceMissingDataLogs.ILHook,
		AddBurdensToElpis.ILHook,
		SaveData.Hook,
		LoadData.Hook,
		LoadContent.Hook,
		ModifyData.Hook,
		ExtendTypeArray.Hook,
		SetFoundState.Hook,
		ModifyUnlocked.Hook,
	];

	internal static Collections.Dictionary<BurdenType, BurdenData> ModdedStore = [];

	internal static Collections.Dictionary<string, BurdenType> NameToType = [];

	internal static Collections.Dictionary<BurdenType, string> TypeToName = [];

	internal static Collections.Dictionary<string, FoundState> NameToFoundState = [];

	internal static int LastType = (int)BurdenType.UnlockNG;

	internal static bool FirstLoad = true;

	/// <summary> </summary>
	public struct Data {
		#pragma warning disable
		public Data() {}
		#pragma warning restore

		/// <summary> </summary>
		public string Name;
		/// <summary> </summary>
		public int MaxBurdenLevel;
		/// <summary> </summary>
		public int InitialBurdenCost;
		/// <summary> </summary>
		public float ScalingBurdenCost;
		/// <summary> </summary>
		public bool Disabled = false;
		/// <summary> </summary>
		public float StatsGain;
		/// <summary> </summary>
		public string Title;
		/// <summary> </summary>
		public string Description;
		/// <summary> </summary>
		public string FlavourText;
		/// <summary> </summary>
		public string Hint;
		/// <summary> </summary>
		public string? IconPath;
		/// <summary> </summary>
		public FoundState DefaultFoundState = FoundState.NotFound;

		/// <summary> </summary>		
		public BurdenData ToScriptableObject() {
			var data = UnityE.ScriptableObject.CreateInstance<BurdenData>();			
			data.Name = Name;
			data.MaxBurdenLevel = MaxBurdenLevel;
			data.InitialBurdenCost = InitialBurdenCost;
			data.ScalingBurdenCost = ScalingBurdenCost;
			data.Disabled = Disabled;
			data.StatsGain = StatsGain;
			data.Title = Title;
			data.Description = Description;
			data.Description2 = FlavourText;
			data.Hint = Hint;
			return data;
		}
	}

	/// <summary>
	/// Registers custom Burdens
	/// </summary>
	/// <param name="data"></param>
	/// <param name="icon"></param>
	/// <param name="found_state">Default <see cref="FoundState"/></param>
	/// <param name="mod_assembly"></param>
	/// <returns></returns>
	public static BurdenType Register(BurdenData data, UnityE.Texture2D? icon = null, FoundState found_state = FoundState.NotFound, Reflect.Assembly? mod_assembly = null) {
		while (IconLibrary.Instance == null) { }

		mod_assembly = mod_assembly ?? Reflect.Assembly.GetCallingAssembly();
		string modName = RL2API.AssemblyToMod[mod_assembly].Manifest.Name;
		string name = modName + "/" + data.Name;

		BurdenType type = NameToType.TryGetValue(name, out BurdenType value) ? value : (BurdenType)(++LastType);
		NameToType[name] = type;
		TypeToName[type] = name;

		ModdedStore[type] = data;

		// Add Burden icons
		UnityE.Sprite burdenSprite = IconLibrary.Instance.m_defaultSprite;
		if (icon != null) {
			burdenSprite = UnityE.Sprite.Create(icon, new UnityE.Rect(0, 0, icon.width / 2, icon.height / 2), new UnityE.Vector2(.5f, .5f));
		}
		IconLibrary.Instance.m_burdenIconLibrary.Add(type, burdenSprite);

		// Manage seen state
		BurdenObj obj = new BurdenObj(type) {
			FoundState = NameToFoundState.TryGetValue(name, out FoundState seen) ? seen : found_state
		};
		SaveManager.PlayerSaveData.BurdenObjTable[type] = obj;
		NameToFoundState[name] = obj.FoundState;

		RL2API.Log($"Saved Burden '{data.Name}' as {type}");
		return type;
	}

	/// <inheritdoc cref="Register(BurdenData, UnityE.Texture2D?, FoundState, Reflect.Assembly?)"/>
	/// <param name="data"></param>
	/// <returns></returns>
	public static BurdenType Register(Data data) {
		var scriptable_object = data.ToScriptableObject();
		return Register(
			scriptable_object, 
			data.IconPath is null ? null : Loader.TextureExtension.LoadTexture(data.IconPath), 
			data.DefaultFoundState,
			Reflect.Assembly.GetCallingAssembly()
		);
	}

	/// <summary>
	/// Retrieves the <see cref="BurdenType"/> of a modded Burden
	/// </summary>
	/// <param name="burdenName">Name of the Burden in the format "Mod Name From Manifest/BurdenName"</param>
	/// <returns><see cref="BurdenType.None"/> if Burden was not found</returns>
	public static BurdenType GetType(string burdenName) {
		BurdenType type = BurdenType.None;
		NameToType.TryGetValue(burdenName, out type);
		return type;
	}

	/// <summary>
	/// Ran when Burden data is being saved. <br></br>
	/// Current save profile canm be accessed via <see cref="SaveManager.CurrentProfile"/>
	/// </summary>
	public static class SaveData {
		/// <inheritdoc cref="SaveData"/>
		public delegate void Definition();

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static MM.Hook Hook = new MM.Hook(
			typeof(SaveManager).GetMethod(nameof(SaveManager.LoadAllGameData), Reflect.BindingFlags.Public | Reflect.BindingFlags.Static),
			Method,
			new MM.HookConfig() {
				ID = "RL2.API::Burdens.SaveData",
				ManualApply = true,
			}
		);

		internal static LOAD_RESULT Method(System.Func<int, bool, LOAD_RESULT> orig, int currentProfile, bool loadAccountData) {
			SaveModdedData();
			Event?.Invoke();
			LOAD_RESULT result = orig(currentProfile, loadAccountData);
			return result;
		}

		internal static void SaveModdedData() {
			// Nothing has been loaded, so there is nothing to save
			if (FirstLoad) return;

			// Save Burden types
			string directory = IO.Path.Combine(SaveFileSystem.PersistentDataPath, "Saves", "RL2.API");
			if (!IO.Directory.Exists(directory)) IO.Directory.CreateDirectory(directory);

			directory = IO.Path.Combine(directory, "Burdens");
			if (!IO.Directory.Exists(directory)) IO.Directory.CreateDirectory(directory);

			string saveTypes = IO.Path.Combine(directory, "Types.json");
			IO.File.WriteAllText(saveTypes, TinyJson.JsonWriter.ToJson(NameToType));

			// Save found state
			directory = IO.Path.Combine(SaveManager.GetSaveDirectoryPath(SaveManager.CurrentProfile, false), "RL2.API");
			if (!IO.Directory.Exists(directory)) IO.Directory.CreateDirectory(directory);

			directory = IO.Path.Combine(directory, "Burdens");
			if (!IO.Directory.Exists(directory)) IO.Directory.CreateDirectory(directory);

			string savedFoundState = IO.Path.Combine(directory, "FoundState.json");
			IO.File.WriteAllText(savedFoundState, TinyJson.JsonWriter.ToJson(NameToFoundState));
		}
	}

	/// <summary>
	/// Ran when Burden save data is being loaded <br></br>
	/// This includes loading saved Burden types if it's the first load, as well as the Burden's 'found' state.
	/// </summary>
	public static class LoadData {
		/// <inheritdoc cref="LoadData"/>
		public delegate void Definition();

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static MM.Hook Hook = new MM.Hook(
			typeof(SaveManager).GetMethod(nameof(SaveManager.LoadAllGameData), Reflect.BindingFlags.Public | Reflect.BindingFlags.Static),
			Method,
			new MM.HookConfig() {
				ID = "RL2.API::Burdens.LoadData",
				ManualApply = true,
			}
		);

		internal static LOAD_RESULT Method(System.Func<int, bool, LOAD_RESULT> orig, int currentProfile, bool loadAccountData) {
			LOAD_RESULT result = orig(currentProfile, loadAccountData);
			NameToFoundState.Clear();
			LoadFoundState();

			if (FirstLoad) LoadSavedTypes();

			Event?.Invoke();

			return result;
		}

		internal static void LoadSavedTypes() {
			string directory = IO.Path.Combine(SaveFileSystem.PersistentDataPath, "Saves", "RL2.API");
			if (!IO.Directory.Exists(directory)) IO.Directory.CreateDirectory(directory);

			directory = IO.Path.Combine(directory, "Burdens");
			if (!IO.Directory.Exists(directory)) IO.Directory.CreateDirectory(directory);

			string savedTypes = IO.Path.Combine(directory, "Types.json");
			if (IO.File.Exists(savedTypes)) {
				NameToType = TinyJson.JsonParser.FromJson<Collections.Dictionary<string, BurdenType>>(IO.File.ReadAllText(savedTypes));
				var sorted = System.Linq.Enumerable.ToList(NameToType.Values);
				sorted.Sort();
				if (sorted.Count > 0)
					LastType = (int)sorted[sorted.Count - 1];
			}
		}

		internal static void LoadFoundState() {
			string directory = IO.Path.Combine(SaveManager.GetSaveDirectoryPath(SaveManager.CurrentProfile, false), "RL2.API");
			if (!IO.Directory.Exists(directory)) IO.Directory.CreateDirectory(directory);

			directory = IO.Path.Combine(directory, "Burdens");
			if (!IO.Directory.Exists(directory)) IO.Directory.CreateDirectory(directory);

			string savedFoundState = IO.Path.Combine(directory, "FoundState.json");
			if (IO.File.Exists(savedFoundState)) {
				NameToFoundState = TinyJson.JsonParser.FromJson<Collections.Dictionary<string, FoundState>>(IO.File.ReadAllText(savedFoundState));
			}
		}
	}

	/// <summary>
	/// Use to register Burdens with <see cref="Burdens.Register(BurdenData, UnityE.Texture2D?, FoundState, Reflect.Assembly?)"/>
	/// </summary>
	/// <remarks>This is ran only once for the entire game session</remarks>
	public static class LoadContent {
		/// <inheritdoc cref="LoadContent"/>
		public delegate void Definition();

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static MM.Hook Hook = new MM.Hook(
			typeof(SaveManager).GetMethod(nameof(SaveManager.LoadAllGameData), Reflect.BindingFlags.Public | Reflect.BindingFlags.Static),
			Method,
			new MM.HookConfig() {
				ID = "RL2.API::Burdens.LoadContent",
				ManualApply = true,
				After = ["RL2.API::Burdens.LoadData"]
			}
		);

		internal static LOAD_RESULT Method(System.Func<int, bool, LOAD_RESULT> orig, int currentProfile, bool loadAccountData) {
			LOAD_RESULT result = orig(currentProfile, loadAccountData);

			if (FirstLoad) Event?.Invoke();
			FirstLoad = false;

			return result;
		}
	}

	/// <summary> 
	///	Allows for modifying Burden data
	/// </summary>
	public static class ModifyData {
		/// <summary> </summary>
		public delegate void Definition(BurdenType type, BurdenData data);

		/// <summary> </summary>
		public static event Definition? Event;

		internal static MM.Hook Hook = new MM.Hook(
			typeof(BurdenLibrary).GetMethod(nameof(BurdenLibrary.GetBurdenData), Reflect.BindingFlags.Public | Reflect.BindingFlags.Static),
			Method,
			new MM.HookConfig() {
				ID = "RL2.API::Burdens.ModifyData",
				ManualApply = true,
			}
		);

		internal static BurdenData Method(System.Func<BurdenType, BurdenData> orig, BurdenType type) {
			var data = orig(type);
			if (data == null) {
				ModdedStore.TryGetValue(type, out data);
			}

			Event?.Invoke(type, data);
			return data;
		}
	}

	/// <summary>
	/// Allows extending the <see cref="BurdenType_RL.TypeArray"/>
	/// </summary>
	public static class ExtendTypeArray {
		/// <inheritdoc cref="ExtendTypeArray"/>
		/// <param name="list"></param>
		public delegate void Definition(ref Collections.List<BurdenType> list);

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static MM.Hook Hook = new MM.Hook(
			typeof(BurdenType_RL).GetProperty(nameof(BurdenType_RL.TypeArray), Reflect.BindingFlags.Public | Reflect.BindingFlags.Static).GetMethod,
			Method,
			new MM.HookConfig() {
				ID = "RL2.API::Burdens.ExtendTypeArray",
				ManualApply = true,
			}
		);

		internal static BurdenType[] Method(System.Func<BurdenType[]> orig) {
			var original = System.Linq.Enumerable.ToList(orig());
			original.AddRange(ModdedStore.Keys);
			Event?.Invoke(ref original);
			return original.ToArray();
		}
	}

	/// <summary>
	/// Allows hijacking of the <see cref="BurdenManager.SetFoundState(BurdenType, FoundState, bool)"/> method
	/// by modifying the provided parameters
	/// </summary>
	public static class SetFoundState {
		/// <inheritdoc cref="SetFoundState"/>
		/// <param name="type"></param>
		/// <param name="state"></param>
		/// <param name="override_values"></param>
		public delegate void Definition(BurdenType type, ref FoundState state, ref bool override_values);
		 
		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static MM.Hook Hook = new MM.Hook(
			typeof(BurdenManager).GetMethod(nameof(BurdenManager.SetFoundState), Reflect.BindingFlags.Public | Reflect.BindingFlags.Static),
			Method,
			new MM.HookConfig() {
				ID = "RL2.API::Burdens.SetFoundState",
				ManualApply = true,
			}
		);

		internal static void Method(System.Action<BurdenType, FoundState, bool> orig, BurdenType type, FoundState state, bool override_values) {
			Event?.Invoke(type, ref state, ref override_values);
			
			if (!TypeToName.TryGetValue(type, out string name)) {
				orig(type, state, override_values);
				return;
			}

			BurdenObj burden = BurdenManager.GetBurden(type);
			if (!burden.IsNativeNull() && (override_values || state > burden.FoundState)) {
				burden.FoundState = state;
				NameToFoundState[name] = state;
			}
		}
	}

	/// <summary>
	/// Allows modifying the "Unlocked" state of a Burden
	/// </summary>
	public static class ModifyUnlocked {
		/// <inheritdoc cref="ModifyUnlocked"/>
		/// <param name="type"></param>
		/// <param name="unlocked"></param>
		public delegate void Definition(BurdenType type, ref bool unlocked);

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static MM.Hook Hook = new MM.Hook(
			typeof(BurdenManager).GetMethod(nameof(BurdenManager.IsBurdenUnlocked), Reflect.BindingFlags.Public | Reflect.BindingFlags.Static),
			Method,
			new MM.HookConfig() {
				
				ID = "RL2.API::Burdens.ModifyUnlocked",
				ManualApply = true,
			}
		);

		internal static bool Method(System.Func<BurdenType, bool> orig, BurdenType type) {
			bool result = orig(type);
			if (TypeToName.TryGetValue(type, out string name)) {
				result = NameToFoundState[name] > FoundState.NotFound && !ModdedStore[type].Disabled;
			}
			Event?.Invoke(type, ref result);

			return result;
		}
	} 

	internal static class AddBurdensToElpis {
		internal static MM.ILHook ILHook = new MM.ILHook(
			typeof(NewGamePlusOmniUIWindowController).GetMethod(nameof(NewGamePlusOmniUIWindowController.CreateEntries), Reflect.BindingFlags.NonPublic | Reflect.BindingFlags.Instance),
			Patch,
			new MM.ILHookConfig() {
				ID = "RL2.API::IL::Burdens.AddBurdensToElpis",
				ManualApply = true,
			}
		);

		internal static void Patch(Cil.ILContext il) {
			var cursor = new Cil.ILCursor(il);

			if (cursor.TryGotoNext(Cil.MoveType.Before, i => Cil.ILPatternMatchingExt.MatchStloc(i, 0))) {
				cursor.EmitDelegate(ModifyEntryArray);
			}
		}

		private static System.Array ModifyEntryArray(System.Array original_array) {
			BurdenType[] new_order = new BurdenType[ModdedStore.Keys.Count + original_array.Length];
			int ng_plus_entry = original_array.Length - 1;

			original_array.CopyTo(new_order, 0);
			ModdedStore.Keys.CopyTo(new_order, ng_plus_entry);

			return new_order;
		}
	}

	internal static class SilenceMissingDataLogs {
		internal static MM.ILHook ILHook = new MM.ILHook(
			typeof(BurdenLibrary).GetMethod(nameof(BurdenLibrary.GetBurdenData), Reflect.BindingFlags.Public | Reflect.BindingFlags.Static),
			Patch,
			new MM.ILHookConfig() {
				ID = "RL2.API::IL::Burdens.SilenceMissingDataLogs",
				ManualApply = true,
			}
		);

		internal static void Patch(Cil.ILContext il) {
			var cursor = new Cil.ILCursor(il);

			if (cursor.TryGotoNext(Cil.MoveType.Before, i => Cil.ILPatternMatchingExt.MatchBrfalse(i, out _))) {
				cursor.Emit(IL.OpCodes.Ldarg_0);
				cursor.EmitDelegate(ModdedStore.ContainsKey);

				cursor.Emit(IL.OpCodes.Or);
			}
		}
	}
}