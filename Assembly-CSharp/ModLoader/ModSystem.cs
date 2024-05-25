using UnityEngine;

namespace RL2.ModLoader;

/// <summary>
/// Constantly active script attached to the <see cref="GameManager"/>'s GameObject.
/// </summary>
public abstract class ModSystem : ModType
{
	/// <summary>
	/// Ran immedieately after loading the ModSystem.
	/// </summary>
	public virtual void OnLoad() { }

	/// <summary>
	/// Allows assigning a custom map icon to a room.
	/// </summary>
	/// <param name="roomToCheck">Room to modify the map icon for</param>
	/// <param name="getUsed">Is the room "finished" (failed/completed Fairy Chest, empty relic room etc.)</param>
	/// <param name="isMergeRoom">Is the room made from multiple rooms</param>
	/// <returns>The texture used as the map icon. Return <see langword="null"/> to follow vanilla logic</returns>
	public virtual Texture2D? ModifyRoomIcon(GridPointManager roomToCheck, bool getUsed, bool isMergeRoom) {
		return null;
	}

	/// <summary>
	/// Determines whether the instance of this class should be initialized.
	/// </summary>
	public virtual bool IsLoadingEnabled() => true;
}
