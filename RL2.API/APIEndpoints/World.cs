using System;
using UnityEngine;

namespace RL2.API;

/// <summary>
/// Provides endpoints for game world related APIs
/// </summary>
public static class World
{
	/// <summary>
	/// Provides endpoints for map related APIs
	/// </summary>
	public static class Map
	{
		/// <summary>
		/// Allows assigning a custom map icon to a room
		/// </summary>
		/// <param name="roomToCheck">Room to modify the map icon for</param>
		/// <param name="getUsed">Is the room "finished" (failed/completed Fairy Chest, empty relic room etc.)</param>
		/// <param name="isMergeRoom">Is the room made out of multiple rooms</param>
		/// <returns>
		/// The texture used as the map icon <br/>	
		/// <see langword="null"/> to follow vanilla logic
		/// </returns>
		public delegate Texture2D? ModifyRoomIcon_delegate(GridPointManager roomToCheck, bool getUsed, bool isMergeRoom);

		/// <inheritdoc cref="ModifyRoomIcon_delegate"/>
		public static event ModifyRoomIcon_delegate? ModifyRoomIcon;

		internal static Texture2D? ModifyRoomIcon_Invoke(GridPointManager roomToCheck, bool getUsed, bool isMergeRoom) {
			if (ModifyRoomIcon is null) {
				return null;
			}

			Texture2D? modifiedIcon = null;
			foreach (Delegate subscriber in ModifyRoomIcon.GetInvocationList()) {
				if (modifiedIcon != null) {
					break;
				}

				modifiedIcon = (Texture2D?)subscriber.DynamicInvoke(roomToCheck, getUsed, isMergeRoom);
			}
			return modifiedIcon;
		}
	}
}