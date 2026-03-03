using MM = MonoMod.RuntimeDetour;
using Cil = MonoMod.Cil;
using IL = MonoMod.Cil.ILPatternMatchingExt;
using Binding = System.Reflection.BindingFlags;
using Collections = System.Collections.Generic;
using IO = System.IO;
using TinyJson = Rewired.Utils.Libraries.TinyJson;

namespace RL2.API;

/// <summary> </summary>
public static class SoulShop {
	internal static MM.IDetour[] Hooks = [
		DataManager.Hook,
		DeleteData.Hook,
		ModifyEntryArray.ILHook,
		ModifyData.Hook,
		ModifyObj.Hook,
		OnOpen.Hook,
		OnClose.Hook,
	];

	/// <summary> Constructable proxy of <see cref="SoulShopData"/> </summary>
	public struct Data
	{
		/// <summary> Internal name of this Soul Shop upgrade </summary>	
		public string Name;
		/// <summary> </summary>
		public bool Disabled;
		/// <summary> Amount of "stats" buying this upgrade for the first time gives </summary>
		/// <remarks> These "stats" aren't actual stats, but rather what is displayed in the description </remarks>j
		public float FirstLevelStatGain;
		/// <summary> Amount of "stats" buying this upgrade rewards the player with from level 2 onwards </summary>
		public float AdditionalLevelStatGain;
		/// <summary> </summary>
		public int Position;
		/// <summary> Base Soul Stone cose </summary>
		public int BaseCost;
		/// <summary> Amount of Souls Stones by which the cost will be increased </summary>
		public int ScalingCost;
		/// <summary> Max level without Cosmic Overload </summary>
		public int MaxLevel;
		/// <summary> How many Soul Shop upgrades the player must buy before unlocking this one </summary>
		public int UnlockLevel;
		/// <summary> Max level with Cosmic Overload </summary>
		public int OverloadMaxLevel;
		/// <summary> </summary>
		public int MaxLevelScalingCap;
		/// <summary> </summary>
		public int MaxSoulCostCap;
		/// <summary> </summary>
		public int TotalCostCapFalseOverloadFalse;
		/// <summary> </summary>
		public int TotalCostCapTrueOverloadFalse;
		/// <summary> </summary>
		public string Title;
		/// <summary> </summary>
		public string Description;
		/// <summary> </summary>
		public string StatsTitle;

		/// <summary>
		/// Turns this proxy into a Scriptable object
		/// </summary>
		/// <returns></returns>
		public SoulShopData ToScriptableObject() {
			var data = UnityEngine.ScriptableObject.CreateInstance<SoulShopData>();
			data.Name = Name;
			data.Disabled = Disabled;
			data.FirstLevelStatGain = FirstLevelStatGain;
			data.AdditionalLevelStatGain = AdditionalLevelStatGain;
			data.Position = Position;
			data.BaseCost = BaseCost;
			data.ScalingCost = ScalingCost;
			data.MaxLevel = MaxLevel;
			data.UnlockLevel = UnlockLevel;
			data.OverloadMaxLevel = OverloadMaxLevel;
			data.MaxLevelScalingCap = MaxLevelScalingCap;
			data.MaxSoulCostCap = MaxSoulCostCap > 0 ? MaxSoulCostCap : (int.MaxValue / 100) * 100;
			data.TotalCostCapFalseOverloadFalse = TotalCostCapFalseOverloadFalse > 0 ? TotalCostCapFalseOverloadFalse : (int.MaxValue / 100) * 100;
			data.TotalCostCapTrueOverloadFalse = TotalCostCapTrueOverloadFalse > 0 ? TotalCostCapTrueOverloadFalse : (int.MaxValue / 100) * 100;
			data.Title = Title;
			data.Description = Description;
			data.StatsTitle = StatsTitle;
			return data;
		}
	}

	/// <summary></summary>
	public struct Entry {
		// /// <summary></summary>
		// public string? IconPath;
		/// <summary></summary>
		public bool IsToggle;

		/// <summary></summary>		
		public delegate void CreationEvent(SoulShopOmniUIEntry entry);
		/// <inheritdoc cref="CreationEvent" />
		public CreationEvent OnCreate;
	}

	[System.Serializable]
	internal class Levels {
		[TinyJson.Serialize] internal int Max;
		[TinyJson.Serialize] internal int Current;
	}

	internal static Collections.Dictionary<SoulShopType, SoulShopData> ModdedStore = [];
	internal static Collections.Dictionary<SoulShopType, Entry> EntryStore = [];
	internal static int LastType = (int)SoulShopType.UnlockOverload + 1;
	internal static Collections.Dictionary<SoulShopType, SoulShopObj> ModdedObjStore = [];
	internal static Collections.Dictionary<string, bool> TypeToSeenState = [];
	internal static Collections.Dictionary<string, Levels> TypeToLevel = [];
	internal static Collections.Dictionary<string, SoulShopType> NameToType = [];
	internal static bool FirstLoad = true;
	
	/// <summary> Registers your custom Soul Shop upgrade </summary>
	/// <param name="data"></param>
	/// <param name="entry"></param>
	/// <param name="icon"></param>
	/// <returns></returns>
	public static SoulShopType Register(Data data, Entry entry, UnityEngine.Texture2D? icon = null) {					
		string name = $"{RL2API.AssemblyToManifest[System.Reflection.Assembly.GetCallingAssembly()].Name}/{data.Name}";
		SoulShopType type = NameToType.TryGetValue(name, out var t) ? t : NameToType[name] = (SoulShopType)(LastType++);
		
		ModdedStore[type] = data.ToScriptableObject();
		(int owned, int equipped) = TypeToLevel.TryGetValue($"{type}", out var levels) ? (levels.Max, levels.Current) : (0,0); 
		if (!TypeToSeenState.TryGetValue($"{type}", out bool viewed)) {
			viewed = false;
		}

		ModdedObjStore[type] = new SoulShopObj(type) { 
			WasViewed = viewed,
			m_equippedLevel = equipped,
			m_ownedLevel = owned,
		};
		EntryStore[type] = entry;
		
		if (icon != null) {
			IconLibrary.Instance.m_soulShopIconLibrary.Add(
				type, 
				UnityEngine.Sprite.Create(icon, new UnityEngine.Rect(0, 0, icon.width, icon.height), new UnityEngine.Vector2(0.5f, 0.5f))
			);
		}

		RL2API.Log($"Saved SoulShop upgrade '{name}' as '{type}'");

		return type;
	}

	/// <summary> </summary>
	/// <param name="name"></param>
	/// <returns></returns>
	public static SoulShopType GetType(string name) {
		return NameToType.TryGetValue(name, out var type) ? type : SoulShopType.None;
	}

	/// <summary> Allows extending the list of Soul Shop unlocks </summary>
	public static class ModifyEntryArray {
		/// <inheritdoc cref="ModifyEntryArray"/>
		public delegate void Definition(ref Collections.List<SoulShopOmniUIEntry> entries);

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static MM.ILHook ILHook = new MM.ILHook(
			typeof(SoulShopOmniUIWindowController).GetMethod(nameof(SoulShopOmniUIWindowController.CreateEntries), Binding.NonPublic | Binding.Instance),
			Patch,
			new MM.ILHookConfig() {
				ID = "RL2.API::IL::SoulShop.ExtendEntryArray",
				ManualApply = true,
			}
		);

		internal static void Patch(Cil.ILContext ctx) {
			var cursor = new Cil.ILCursor(ctx);

			if (cursor.TryGotoNext(Cil.MoveType.Before, ins => IL.MatchStloc(ins, 1))) {
				cursor.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_0);
				cursor.EmitDelegate(Method);

				static SoulShopOmniUIEntry[] Method(SoulShopOmniUIEntry[] original, SoulShopOmniUIWindowController self) {
					var list = System.Linq.Enumerable.ToList(original);
					foreach (SoulShopType type in ModdedStore.Keys) {	
						var extraData = EntryStore[type];
						var entry = UnityEngine.GameObject.Instantiate(self.m_entryPrefab);
						entry.gameObject.SetActive(true);
						if (extraData.IsToggle) {
							entry.m_isToggle = true;
							entry.m_toggleButton.SoulShopType = type;
							entry.m_toggleButton.ParentEntry = entry;
						}
						foreach (OmniUIButton button in entry.gameObject.GetComponentsInChildren<OmniUIButton>()) {
							if (button is not SoulShopOmniUIEquipButton) continue;
							button.gameObject.SetActive(false);
							button.gameObject.GetComponentInParent<UnityEngine.RectTransform>()?
								.gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>()?
								.gameObject.SetActive(false);
						}
						entry.transform.localScale *= 0.665f;
						entry.m_soulShopType = type;
						entry.transform.SetParent(self.EntryLayoutGroup.transform);
						extraData.OnCreate?.Invoke(entry);
						list.Add(entry);
					}
					
					Event?.Invoke(ref list);
					self.EntryArray = list.ToArray();
					return self.EntryArray;
				}
			}

		}
	}

	/// <summary></summary>	
	public static class ModifyData {
		/// <inheritdoc cref="ModifyData" />
		public delegate void Definition(SoulShopType type, SoulShopData data);

		/// <inheritdoc cref="Definition" />
		public static event Definition? Event;

		internal static MM.Hook Hook = new MM.Hook(
			typeof(SoulShopLibrary).GetMethod(nameof(SoulShopLibrary.GetSoulShopData), Binding.Public | Binding.Static),
			Method,
			new MM.HookConfig() {
				ID = "RL2.API::SoulShop.ModifyData",
				ManualApply = true,
			}
		);

		internal static SoulShopData Method(System.Func<SoulShopType, SoulShopData> orig, SoulShopType type) {
			var data = orig(type);
			if (data == null) {
				ModdedStore.TryGetValue(type, out data);
			}

			Event?.Invoke(type, data);
			return data;
		}
	}

	/// <summary>
	/// Allows modifying the <see cref="SoulShopObj"/> tied to Soul Shop upgrades
	/// </summary>
	public static class ModifyObj {
		/// <inheritdoc cref="ModifyObj" />
		public delegate void Definition(SoulShopType type, SoulShopObj obj);

		/// <inheritdoc cref="Definition" />
		public static event Definition? Event;

		internal static MM.Hook Hook = new MM.Hook(
			typeof(ModeSaveData).GetMethod(nameof(ModeSaveData.GetSoulShopObj), Binding.Public | Binding.Instance),
			Method,
			new MM.HookConfig() {
				ID = "RL2.API::SoulShop.ModifyObj",
				ManualApply = true,
			}
		);

		internal static SoulShopObj Method(System.Func<ModeSaveData, SoulShopType, SoulShopObj> orig, ModeSaveData self, SoulShopType type) {
			var obj = orig(self, type);
			if (obj == null) {
				ModdedObjStore.TryGetValue(type, out obj);
			}
			
			Event?.Invoke(type, obj);
			return obj;
		}
	}

	/// <summary>
	/// Ran when SoulShop data is being saved. <br></br>
	/// Current save profile canm be accessed via <see cref="RL2.API.Hooks.PreviousProfile"/>
	/// </summary>
	public static class SaveData {
		/// <inheritdoc cref="SaveData"/>
		public delegate void Definition();

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static void SaveModdedData() {
			// Save SoulShop types
			string directory = IO.Path.Combine(SaveFileSystem.PersistentDataPath, "Saves", "RL2.API");
			if (!IO.Directory.Exists(directory)) IO.Directory.CreateDirectory(directory);

			directory = IO.Path.Combine(directory, "SoulShop");
			if (!IO.Directory.Exists(directory)) IO.Directory.CreateDirectory(directory);

			string saveTypes = IO.Path.Combine(directory, "Types.json");
			IO.File.WriteAllText(saveTypes, TinyJson.JsonWriter.ToJson(NameToType));

			// Save found state
			directory = IO.Path.Combine(SaveManager.GetSaveDirectoryPath(API.Hooks.PreviousProfile, false), "RL2.API");
			if (!IO.Directory.Exists(directory)) IO.Directory.CreateDirectory(directory);

			directory = IO.Path.Combine(directory, "SoulShop");
			if (!IO.Directory.Exists(directory)) IO.Directory.CreateDirectory(directory);

			string savedSeenState = IO.Path.Combine(directory, "SeenState.json");

			foreach (var kvp in ModdedObjStore) {
				TypeToLevel[$"{kvp.Key}"] = new Levels { Max = kvp.Value.m_ownedLevel, Current = kvp.Value.m_equippedLevel };
				TypeToSeenState[$"{kvp.Key}"] = kvp.Value.WasViewed;
			}

			IO.File.WriteAllText(savedSeenState, TinyJson.JsonWriter.ToJson(TypeToSeenState));
		
			// Save levels
			string savedLevels = IO.Path.Combine(directory, "Levels.json");
			IO.File.WriteAllText(savedLevels, TinyJson.JsonWriter.ToJson(TypeToLevel));
		}

		internal static void Invoke() => Event?.Invoke();
	}

	/// <summary> </summary>
	public static class DeleteData {
		/// <inheritdoc cref="DeleteData" />
		/// <param name="save_batch"></param>
		/// <param name="profile"></param>
		/// <param name="save_type"></param>
		public delegate void Definition(SaveFileSystem.SaveBatch save_batch, int profile, SaveDataType save_type);

		/// <inheritdoc cref="Definition" />
		public static event Definition? Event;

		internal static MM.Hook Hook = new MM.Hook(
			typeof(SaveManager).GetMethod(nameof(SaveManager.DeleteSaveFile), Binding.Public | Binding.Static),
			Wrapper,
			new MM.HookConfig() {
				ID = "RL2.API::Burdens.DeleteData",
				ManualApply = true,
			}
		);

		static void Wrapper(Definition orig, SaveFileSystem.SaveBatch batch, int profile, SaveDataType save_type) {
			orig(batch, profile, save_type);
			DeleteModdedData(profile, save_type);
			Event?.Invoke(batch, profile, save_type);
		}

		internal static void DeleteModdedData(int profile, SaveDataType save_type) {
			if (save_type != SaveDataType.Player) return;	

			string saved_path = IO.Path.Combine(SaveManager.GetSaveDirectoryPath(profile, false), "RL2.API", "SoulShop ", "SeenState.json");
			if (IO.File.Exists(saved_path)) {
				IO.File.Delete(saved_path);
			}

			saved_path = IO.Path.Combine(SaveManager.GetSaveDirectoryPath(profile, false), "RL2.API", "SoulShop ", "Levels.json");
			if (IO.File.Exists(saved_path)) {
				IO.File.Delete(saved_path);
			}			
		}
	}

	/// <summary>
	/// Ran when SoulShop save data is being loaded <br></br>
	/// This includes loading saved SoulShop types if it's the first load, as well as the SoulShop s 'found' state.
	/// </summary>
	public static class LoadData {
		/// <inheritdoc cref="LoadData"/>
		public delegate void Definition();

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static void LoadSavedTypes() {
			string directory = IO.Path.Combine(SaveFileSystem.PersistentDataPath, "Saves", "RL2.API");
			if (!IO.Directory.Exists(directory)) IO.Directory.CreateDirectory(directory);

			directory = IO.Path.Combine(directory, "SoulShop");
			if (!IO.Directory.Exists(directory)) IO.Directory.CreateDirectory(directory);

			string savedTypes = IO.Path.Combine(directory, "Types.json");
			if (IO.File.Exists(savedTypes)) {
				NameToType = TinyJson.JsonParser.FromJson<Collections.Dictionary<string, SoulShopType>>(IO.File.ReadAllText(savedTypes));
				var sorted = System.Linq.Enumerable.ToList(NameToType.Values);
				sorted.Sort();
				if (sorted.Count > 0)
					LastType = (int)sorted[sorted.Count - 1];
			}
		}

		internal static void LoadSeenState() {
			string directory = IO.Path.Combine(SaveManager.GetSaveDirectoryPath(SaveManager.CurrentProfile, false), "RL2.API");
			if (!IO.Directory.Exists(directory)) IO.Directory.CreateDirectory(directory);

			directory = IO.Path.Combine(directory, "SoulShop");
			if (!IO.Directory.Exists(directory)) IO.Directory.CreateDirectory(directory);

			string savedSeenState = IO.Path.Combine(directory, "SeenState.json");
			if (IO.File.Exists(savedSeenState)) {
				var json = IO.File.ReadAllText(savedSeenState);
				TypeToSeenState = TinyJson.JsonParser.FromJson<Collections.Dictionary<string, bool>>(json);
				
				if (FirstLoad) return;
				foreach ((var key, var value) in TypeToSeenState) {
					ModdedObjStore[(SoulShopType)int.Parse(key)].WasViewed = value;
				}
			}
		}

		internal static void LoadLevels() {
			RL2API.Log($"Current: {SaveManager.CurrentProfile}");
			string savedLevels = IO.Path.Combine(SaveManager.GetSaveDirectoryPath(SaveManager.CurrentProfile, false), "RL2.API", "SoulShop", "Levels.json");
			if (IO.File.Exists(savedLevels)) {
				TypeToLevel = TinyJson.JsonParser.FromJson<Collections.Dictionary<string, Levels>>(IO.File.ReadAllText(savedLevels));
				
				foreach ((var key, var value) in TypeToLevel) {
					RL2API.Log($"{key}: Owned {value.Max} Set {value.Current}");
					ModdedObjStore[(SoulShopType)int.Parse(key)].m_equippedLevel = value.Current;
					ModdedObjStore[(SoulShopType)int.Parse(key)].m_ownedLevel = value.Max;
				}
			}
		}

		internal static void Invoke() => Event?.Invoke();
	}

	/// <summary>
	/// Use to register SoulShop  with <see cref="SoulShop.Register(Data, Entry, UnityEngine.Texture2D?)"/>
	/// </summary>
	/// <remarks>This is ran only once for the entire game session</remarks>
	public static class LoadContent {
		/// <inheritdoc cref="LoadContent"/>
		public delegate void Definition();

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static void Invoke() => Event?.Invoke();
	}

	internal static class DataManager {
		internal static MM.Hook Hook = new MM.Hook(
			typeof(SaveManager).GetMethod(nameof(SaveManager.LoadAllGameData), Binding.Public | Binding.Static),
			Wrapper,
			new MM.HookConfig() {
				ID = "RL2.API::SoulShop.LoadManager"
			}
		);

		internal static LOAD_RESULT Wrapper(System.Func<int, bool, LOAD_RESULT> orig, int profile, bool backup) {
			if (!FirstLoad) {
				SaveData.SaveModdedData();
				SaveData.Invoke();
			}
			LOAD_RESULT res = orig(profile, backup);
			if (FirstLoad) LoadData.LoadSavedTypes();
			LoadData.LoadSeenState();
			if (FirstLoad) LoadContent.Invoke();
			LoadData.LoadLevels();
			LoadData.Invoke();

			FirstLoad = false;
			API.Hooks.PreviousProfile = profile;
			return res;
		}
	}
	
	/// <summary> Triggered when the Soul Shop window is opened</summary>
	public static class OnOpen {
		/// <inheritdoc cref="OnOpen"/>
		public delegate void Definition(SoulShopOmniUIWindowController shop);

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static MM.Hook Hook = new MM.Hook(
			typeof(SoulShopOmniUIWindowController).GetMethod(nameof(SoulShopOmniUIWindowController.OnOpen), Binding.NonPublic | Binding.Instance),
			Method,
			new MM.HookConfig() {
				ID = "RL2.API::SoulShop.OnOpen",
				ManualApply = true,
			}
		);

		internal static void Method(System.Action<SoulShopOmniUIWindowController> orig, SoulShopOmniUIWindowController self) {
			orig(self);
			Event?.Invoke(self);
		}
	}
	
	/// <summary> Triggered when the Soul Shop window is closed </summary>
	public static class OnClose {
		/// <inheritdoc cref="OnClose"/>
		public delegate void Definition(SoulShopOmniUIWindowController shop);

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static MM.Hook Hook = new MM.Hook(
			typeof(SoulShopOmniUIWindowController).GetMethod(nameof(SoulShopOmniUIWindowController.OnClose), Binding.NonPublic | Binding.Instance),
			Method,
			new MM.HookConfig() {
				ID = "RL2.API::SoulShop.OnClose",
				ManualApply = true,
			}
		);

		internal static void Method(System.Action<SoulShopOmniUIWindowController> orig, SoulShopOmniUIWindowController self) {
			orig(self);
			Event?.Invoke(self);
		}
	}
}