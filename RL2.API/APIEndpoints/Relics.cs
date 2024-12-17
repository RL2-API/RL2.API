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
			relicSprite = Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height),new Vector2(.5f, .5f));
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
}