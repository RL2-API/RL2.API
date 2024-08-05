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
	/// <param name="characterData">The generated heirs <see cref="CharacterData" /></param>
	/// <param name="classLocked">Whether the heirs class was locked by a Soul Shop ugrade</param>
	/// <param name="spellLocked">Whether the heirs spell was locked by a Soul Shop ugrade</param>
	public virtual void ModifyGeneratedCharacterData(CharacterData characterData, bool classLocked, bool spellLocked) { }


	/// <summary>
	/// Allows modifying look data of generated heirs
	/// </summary>
	/// <param name="lookData">The generated heirs <see cref="PlayerLookController" /></param>
	/// <param name="characterData">The generated heirs <see cref="CharacterData" /></param>
	public virtual void ModifyGeneratedCharacterLook(PlayerLookController lookData, CharacterData characterData) { }

	/// <summary>
	/// Allows modifying character data during character randomization by either the Contrarian trait or by the use of the Transmogrifier<br></br> 
	/// Ran at the end of <see cref="CharacterCreator.ApplyRandomizeKitTrait"/>
	/// </summary>
	/// <param name="characterData">The randomized heirs CharacterData</param>
	public virtual void ModifyCharacterRandomization(CharacterData characterData) { }

	/// <summary>
	/// Allows modifying ability data
	/// </summary>
	/// <param name="type">The queried ability</param>
	/// <param name="data">Returned data of the ability</param>
	public virtual void ModifyAbilityData(AbilityType type, AbilityData data) {	}

	/// <summary>
	/// Determines whether the instance of this class should be initialized
	/// </summary>
	public virtual bool IsLoadingEnabled() => true;
}
