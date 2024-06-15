using UnityEngine;

namespace RL2.ModLoader;

/// <summary>
/// Constantly active script attached to the <see cref="GameManager"/>'s GameObject
/// </summary>
public abstract class ModSystem : ModType
{
	/// <summary>
	/// Ran immedieately after loading the ModSystem
	/// </summary>
	public virtual void OnLoad() { }

	/// <summary>
	/// Allows assigning a custom map icon to a room
	/// </summary>
	/// <param name="roomToCheck">Room to modify the map icon for</param>
	/// <param name="getUsed">Is the room "finished" (failed/completed Fairy Chest, empty relic room etc.)</param>
	/// <param name="isMergeRoom">Is the room made from multiple rooms</param>
	/// <returns>
	/// The texture used as the map icon.<br/>
	/// Return <see langword="null"/> to follow vanilla logic
	/// </returns>
	public virtual Texture2D? ModifyRoomIcon(GridPointManager roomToCheck, bool getUsed, bool isMergeRoom) {
		return null;
	}

	/// <summary>
	/// Allows modifying character data of generated heirs
	/// </summary>
	/// <param name="characterData">The generated heirs CharacterData</param>
	/// <remarks>Currently broken; DO NOT USE</remarks>
	public virtual void ModifyGeneratedCharacter(CharacterData characterData) { }

	/// <summary>
	/// Allows modifying character data during character randomization by either the Contrarian trait or by the use of the Transmogrifier<br></br> 
	/// Ran at the end of <see cref="CharacterCreator.ApplyRandomizeKitTrait"/>
	/// </summary>
	/// <param name="characterData">The randomized heirs CharacterData</param>
	public virtual void ModifyCharacterRandomization(CharacterData characterData) { }

	/// <summary>
	/// Determines whether the instance of this class should be initialized
	/// </summary>
	public virtual bool IsLoadingEnabled() => true;
}
