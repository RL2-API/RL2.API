/*
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RL2.ModLoader;

public partial class RL2API
{
	/// <summary>
	/// Stores map icons as prefabs, accessed by a 
	/// </summary>
	public static Dictionary<string, GameObject> TextureHashToPrefab = new Dictionary<string, GameObject>();

	/// <summary>
	/// Handles modifying the rooms icon on the map
	/// </summary>
	internal static Hook ModifyRoomIcon = new Hook(
		typeof(MapController).GetMethod("GetSpecialIconPrefab", BindingFlags.Public | BindingFlags.Static),
		(Func<GridPointManager, bool, bool, GameObject> orig, GridPointManager roomToCheck, bool getUsed, bool isMergeRoom) => {
			Texture2D? modMapIconTexture = null;
			foreach (ModSystem modSystem in GameManager.Instance.GetComponents<ModSystem>()) {
				if (modMapIconTexture != null) {
					break;
				}
				modMapIconTexture = modSystem.ModifyRoomIcon(roomToCheck, getUsed, isMergeRoom);
			}
			if (modMapIconTexture != null) {
				string textureHash = string.Concat(modMapIconTexture.GetPixels32());
				if (TextureHashToPrefab.ContainsKey(textureHash)) {
					return TextureHashToPrefab[textureHash];
				}
				GameObject modMapIconObject = UnityEngine.Object.Instantiate(MapController.m_instance.m_specialRoomIconPrefab);
				Sprite sprite = Sprite.Create(modMapIconTexture, new Rect(0f, 0f, modMapIconTexture.width, modMapIconTexture.height), new Vector2(.5f, .5f));
				modMapIconObject.GetComponent<SpriteRenderer>().sprite = sprite;
				UnityEngine.Object.DontDestroyOnLoad(modMapIconObject);
				modMapIconObject.SetActive(false);
				TextureHashToPrefab.Add(textureHash, modMapIconObject);
				return modMapIconObject;
			}
			return orig(roomToCheck, getUsed, isMergeRoom);
		}
	);

	internal static Hook ModifyAbilityDataHook = new Hook(
		typeof(AbilityLibrary).GetMethod("GetAbility", BindingFlags.Public | BindingFlags.Static),
		(Func<AbilityType, BaseAbility_RL> orig, AbilityType type) => {
			BaseAbility_RL? ability = orig(type);
			if (ability == null) {
				return ability;
			}

			foreach (ModSystem modSystem in GameManager.Instance.GetComponents<ModSystem>()) {
				modSystem.ModifyAbilityData(type, ability.AbilityData);
			}

			return ability;
		}
	);
}
*/