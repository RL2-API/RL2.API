using JetBrains.Annotations;
using Rewired.Utils.Libraries.TinyJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RL2.API;

/// <summary>
///	Provides endpoints for Relic related APIs
/// </summary>
public static class Relics {
	/// <summary> 
	/// Stores custom relics 
	/// </summary>
	internal static Dictionary<RelicType, RelicData> CustomRelicStore = [];

	internal static Dictionary<string, RelicType> SavedRelicIDs = [];

	internal static int LastRelicID = (int)RelicType.DeathMark;

	/// <summary>
	/// Registers the custom relic
	/// </summary>
	/// <param name="data">Data of the custom relic</param>
	/// <param name="icon">Small icon - displayed in the HUD</param>
	/// <param name="iconBig">Big icon - displayed when picking up the relic</param>
	public static RelicType Register(RelicData data, Texture2D? icon = null, Texture2D? iconBig = null) {
		while (IconLibrary.Instance == null) { }

		Type modType = Assembly.GetCallingAssembly().GetTypes().Where((type) => type.IsSubclassOf(typeof(Mod))).FirstOrDefault();
		string modName = RL2API.GetModInstance(modType)!.Manifest.Name;
		string name = modName + data.Name;

		RelicType ID = SavedRelicIDs.TryGetValue(name, out RelicType value) ? value : (RelicType)(++LastRelicID);
		SavedRelicIDs[name] = ID;

		CustomRelicStore[ID] = data;
		
		// Add regular relic icon
		Sprite relicSprite = IconLibrary.Instance.m_defaultSprite;
		if (icon != null) {
			relicSprite = Sprite.Create(icon, new Rect(0, 0, icon.width/2, icon.height/2), new Vector2(.5f, .5f));
		}
		IconLibrary.Instance.m_relicIconLibrary.Add(ID, relicSprite);

		Sprite relicSpriteBig = IconLibrary.Instance.m_defaultSprite;
		if (iconBig != null) {
			relicSpriteBig = Sprite.Create(iconBig, new Rect(0, 0, iconBig.width, iconBig.height),new Vector2(.5f, .5f));
		}
		IconLibrary.Instance.m_relicLargeIconLibrary.Add(ID, relicSpriteBig);
		
		SaveManager.PlayerSaveData.RelicObjTable[ID] = new RelicObj(ID);

		RL2API.Log($"Saved {data.Name} as {ID}");
		return ID;
	}

	/// <summary>
	/// Gets the RelicType of a custom relic
	/// </summary>
	/// <param name="name">Name of the searched relic in the 'ModName/RelicName' format</param>
	/// <returns><see cref="RelicType"/> if relic is found, <see langword="null"/> if not found </returns>
	public static RelicType? GetRelicType(string name) {
		if (SavedRelicIDs.TryGetValue(name, out RelicType value)) return value;
		return null;
	}

 	/// <summary> 
	///	Allows modifying relic data
	/// </summary>
	/// <param name="type"> Relic type </param>
	/// <param name="data"> Relic data </param>
	public delegate void ModifyData_delegate(RelicType type, RelicData data);

	/// <inheritdoc href="ModifyData_delegate" />
	public static event ModifyData_delegate? ModifyData;

	internal static void ModifyData_Invoke(RelicType type, RelicData data) {
		ModifyData?.Invoke(type, data);
	}

	/// <summary>
	/// Allows running code after relic level change
	/// </summary>
	/// <param name="type">Relic type</param>
	/// <param name="levelChange">Change of the relics level (how many copies of the relic the player picked up)</param>
	public delegate void ApplyRelic_delegate(RelicType type, int levelChange);

	/// <inheritdoc cref="ApplyRelic_delegate" />
	public static event ApplyRelic_delegate? ApplyRelic;

	internal static void ApplyRelic_Invoke(RelicType type, int levelChange) {
		ApplyRelic?.Invoke(type, levelChange);
	}

	/// <summary>
	/// Used to disable additional relic functionality when the relic is taken away
	/// </summary>
	/// <param name="type"></param>
	public delegate void StopRelic_delegate(RelicType type);

	/// <inheritdoc cref="StopRelic_delegate" />
	public static event StopRelic_delegate? StopRelic;

	internal static void StopRelic_Invoke(RelicType type) {
		StopRelic?.Invoke(type);
	}

	internal static void LoadSavedData() {
		string path = ModLoader.ModLoader.ModPath + "\\SavedData";
		if (!Directory.Exists(ModLoader.ModLoader.ModPath + "\\SavedData")) Directory.CreateDirectory(ModLoader.ModLoader.ModPath + "\\SavedData");
		
		path = path + "\\RelicIDs.json";
		if (!File.Exists(path)) return;

		SavedRelicIDs = JsonParser.FromJson<Dictionary<string, RelicType>>(File.ReadAllText(path));
		var sorted = SavedRelicIDs.Values.ToList();
		sorted.Sort();
		LastRelicID = (int)sorted.LastOrDefault();
	}

	internal static void SaveData() {
		string path = ModLoader.ModLoader.ModPath + "\\SavedData\\RelicIDs.json";
		File.WriteAllText(path, JsonWriter.ToJson(SavedRelicIDs));
	}
}