using System.Collections.Generic;
using UnityEngine;

namespace RL2.API;

/// <summary>
///	Provides endpoints for Relic related APIs
/// </summary>
public static class Relics {
	/// <summary> 
	/// Stores custom relics 
	/// </summary>
	internal static Dictionary<int, RelicData> CustomRelicStore = [];

	internal static int LastRelicID = (int)RelicType.DeathMark;

	/// <summary>
	/// Registers the custom relic
	/// </summary>
	/// <param name="data">Data of the custom relic</param>
	/// <param name="icon">Small icon - displayed in the HUD</param>
	/// <param name="iconBig">Big icon - displayed when picking up the relic</param>
	public static int Register(RelicData data, Texture2D? icon = null, Texture2D? iconBig = null) {
		while (IconLibrary.Instance == null) {}
		LastRelicID += 1;
		CustomRelicStore[LastRelicID] = data;
		
		// Add regular relic icon
		Sprite relicSprite = IconLibrary.Instance.m_defaultSprite;
		if (icon != null) {
			relicSprite = Sprite.Create(icon, new Rect(0, 0, icon.width/2, icon.height/2),new Vector2(.5f, .5f));
		}
		IconLibrary.Instance.m_relicIconLibrary.Add((RelicType)LastRelicID, relicSprite);

		Sprite relicSpriteBig = IconLibrary.Instance.m_defaultSprite;
		if (iconBig != null) {
			relicSpriteBig = Sprite.Create(iconBig, new Rect(0, 0, iconBig.width, iconBig.height),new Vector2(.5f, .5f));
		}
		IconLibrary.Instance.m_relicLargeIconLibrary.Add((RelicType)LastRelicID, relicSpriteBig);
		
		SaveManager.PlayerSaveData.RelicObjTable[(RelicType)LastRelicID] = new RelicObj((RelicType)LastRelicID);

		return LastRelicID;
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
}