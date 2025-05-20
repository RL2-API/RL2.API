using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RL2.API;

public partial class RL2API
{
	/// <summary>
	/// Stores map icons as prefabs, accessed by a 
	/// </summary>
	internal static Dictionary<string, GameObject> MapIconTextureHashToPrefab = new Dictionary<string, GameObject>();

	/// <summary>
	/// Handles modifying the rooms icon on the map
	/// </summary>
	internal static Hook ModifyRoomIcon = new Hook(
		typeof(MapController).GetMethod("GetSpecialIconPrefab", BindingFlags.Public | BindingFlags.Static),
		(Func<GridPointManager, bool, bool, GameObject> orig, GridPointManager roomToCheck, bool getUsed, bool isMergeRoom) => {
			Texture2D? modMapIconTexture = Map.ModifyRoomIcon_Invoke(roomToCheck, getUsed, isMergeRoom);
			if (modMapIconTexture != null) {
				string textureHash = string.Concat(modMapIconTexture.GetPixels32());
				if (MapIconTextureHashToPrefab.ContainsKey(textureHash)) {
					return MapIconTextureHashToPrefab[textureHash];
				}
				GameObject modMapIconObject = UnityEngine.Object.Instantiate(MapController.m_instance.m_specialRoomIconPrefab);
				Sprite sprite = Sprite.Create(modMapIconTexture, new Rect(0f, 0f, modMapIconTexture.width, modMapIconTexture.height), new Vector2(.5f, .5f));
				modMapIconObject.GetComponent<SpriteRenderer>().sprite = sprite;
				UnityEngine.Object.DontDestroyOnLoad(modMapIconObject);
				modMapIconObject.SetActive(false);
				MapIconTextureHashToPrefab.Add(textureHash, modMapIconObject);
				return modMapIconObject;
			}
			return orig(roomToCheck, getUsed, isMergeRoom);
		}
	);
}