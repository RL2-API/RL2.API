using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RL2.API;

/// <summary>
/// Provides endpoints for map related APIs
/// </summary>
public static class Map
{
	internal static IDetour[] Hooks = [
		ModifyRoomIcon.Hook
	];

	internal static Dictionary<string, GameObject> MapIconTextureHashToPrefab = new Dictionary<string, GameObject>();

	/// <summary>
	/// Allows assigning a custom map icon to a room
	/// </summary>
	public static class ModifyRoomIcon
	{
		/// <inheritdoc cref="ModifyRoomIcon" />
		/// <param name="roomToCheck">Room to modify the map icon for</param>
		/// <param name="getUsed">Is the room "finished" (failed/completed Fairy Chest, empty relic room etc.)</param>
		/// <param name="isMergeRoom">Is the room made out of multiple rooms</param>
		/// <returns>
		/// The texture used as the map icon <br/>	
		/// <see langword="null"/> to follow vanilla logic
		/// </returns>
		public delegate Texture2D? Definition(GridPointManager roomToCheck, bool getUsed, bool isMergeRoom);

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static Hook Hook = new Hook(
			typeof(MapController).GetMethod("GetSpecialIconPrefab", BindingFlags.Public | BindingFlags.Static),
			Method,
			new HookConfig() {
				ID = "RL2.API::Map.ModifyRoomIcon",
				ManualApply = true,
			}
		);

		internal static GameObject Method(Func<GridPointManager, bool, bool, GameObject> orig, GridPointManager roomToCheck, bool getUsed, bool isMergeRoom) {
			GameObject gameObject = orig(roomToCheck, getUsed, isMergeRoom);

			Texture2D? modMapIconTexture = null;
			foreach (Delegate subscriber in Event?.GetInvocationList() ?? []) {
				if (modMapIconTexture != null) {
					break;
				}
				modMapIconTexture = (Texture2D?)subscriber.DynamicInvoke(roomToCheck, getUsed, isMergeRoom);
			}

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

			return gameObject;
		}
	}
}