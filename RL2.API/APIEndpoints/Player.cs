using System;
using UnityEngine;

namespace RL2.API;

/// <summary>
/// Provides endpoints for player related APIs
/// </summary>
public static class Player
{
	/// <summary>
	/// Ran on player spawn
	/// </summary>
	/// <param name="player">The instance of the player that was spawned</param>
	public delegate void OnSpawn_delegate(PlayerController player);

	/// <inheritdoc cref="OnSpawn_delegate"/>
	public static event OnSpawn_delegate? OnSpawn;

	internal static void OnSpawn_Invoke(PlayerController player) {
		OnSpawn?.Invoke(player);
	}

	#region Death
	/// <summary>
	/// Determines whether the affected player should die.
	/// </summary>
	/// <param name="player">Affected player</param>
	/// <param name="killer">GameObject responsible for the player's death</param>
	/// <returns>If the player should die</returns>
	public delegate bool PreKill_delegate(PlayerController player, GameObject killer);

	/// <inheritdoc cref="PreKill_delegate"/>
	public static event PreKill_delegate? PreKill;

	internal static bool PreKill_Invoke(PlayerController player, GameObject killer) {
		if (PreKill is null) {
			return true;
		}

		bool result = true;
		foreach (Delegate subscriber in PreKill.GetInvocationList()) {
			if (!result) {
				break;
			}

			result &= (bool)subscriber.DynamicInvoke(player, killer);
		}
		return result;
	}

	/// <summary>
	/// Ran after the player dies
	/// </summary>
	/// <param name="player">Affected player</param>
	/// <param name="killer">GameObject responsible for the player's death</param>
	public delegate void OnKill_delegate(PlayerController player, GameObject killer);

	/// <inheritdoc cref="OnKill_delegate"/>
	public static event OnKill_delegate? OnKill;

	internal static void OnKill_Invoke(PlayerController player, GameObject killer) {
		OnKill?.Invoke(player, killer);
	}
	#endregion

	/// <summary>
	/// Executed after all stats update. Use to perform stat increases.
	/// </summary>
	/// <param name="player">The player</param>
	public delegate void PostUpdateStats_delegate(PlayerController player);

	/// <inheritdoc cref="PostUpdateStats_delegate" />
	public static event PostUpdateStats_delegate? PostUpdateStats;

	internal static void PostUpdateStats_Invoke(PlayerController player) {
		PostUpdateStats?.Invoke(player);
	}

	/// <summary>
	/// Stores events related to heir generation
	/// </summary>
	public static class HeirGeneration
	{
		/// <summary>
		/// Used to modify character data after character randomization by either the Contrarian trait or by the use of the Transmogrifier<br></br> 
		/// Ran at the end of <see cref="CharacterCreator.ApplyRandomizeKitTrait"/>
		/// </summary>
		/// <param name="data">The randomized heirs CharacterData</param>
		public delegate void ModifyCharacterRandomization_delegate(CharacterData data);

		/// <inheritdoc cref="ModifyCharacterRandomization_delegate"/>
		public static event ModifyCharacterRandomization_delegate? ModifyCharacterRandomization;
		
		internal static void ModifyCharacterRandomization_Invoke(CharacterData data) {
			ModifyCharacterRandomization?.Invoke(data);
		}

		/// <summary>
		/// Used to modify character data of generated heirs
		/// </summary>
		/// <param name="data">The generated heirs <see cref="CharacterData" /></param>
		/// <param name="classLocked">Whether the heirs class was locked by a Soul Shop ugrade</param>
		/// <param name="spellLocked">Whether the heirs spell was locked by a Soul Shop ugrade</param>
		public delegate void ModifyCharacterData_delegate(CharacterData data, bool classLocked, bool spellLocked);

		/// <inheritdoc cref="ModifyCharacterData_delegate"/>
		public static event ModifyCharacterData_delegate? ModifyCharacterData;
		
		internal static void ModifyCharacterData_Invoke(CharacterData data, bool classLocked, bool spellLocked) {
			ModifyCharacterData?.Invoke(data, classLocked, spellLocked);
		}

		/// <summary>
		/// Used to modify look data of generated heirs
		/// </summary>
		/// <param name="lookData">The generated heirs <see cref="PlayerLookController" /></param>
		/// <param name="data">The generated heirs <see cref="CharacterData" /></param>
		public delegate void ModifyCharacterLook_delegate(PlayerLookController lookData, CharacterData data);

		/// <inheritdoc cref="ModifyCharacterLook_delegate"/>
		public static event ModifyCharacterLook_delegate? ModifyCharacterLook;
		
		internal static void ModifyCharacterLook_Invoke(PlayerLookController lookData, CharacterData data) {
			ModifyCharacterLook?.Invoke(lookData, data);
		}
	}

	/// <summary>
	/// Stores events related to player abilities (weapons, spells, talents)
	/// </summary>
	public static class Ability
	{
		/// <summary>
		/// Used to modify ability data
		/// </summary>
		/// <param name="type">The queried ability</param>
		/// <param name="data">Returned data of the ability</param>
		public delegate void ModifyData_delegate(AbilityType type, AbilityData data);

		/// <inheritdoc cref="ModifyData_delegate"/>
		public static event ModifyData_delegate? ModifyData;

		internal static void ModifyData_Invoke(AbilityType type, AbilityData data) {
			ModifyData?.Invoke(type, data);
		}
	}
}