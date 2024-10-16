using System;
using System.Security.Policy;
using UnityEngine;

namespace RL2.ModLoader.API;

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
	/// Stores events used to modify player stats
	/// </summary>
	public static class Stats
	{
		#region Resolve
		/// <summary>
		/// Used to change Resolve by a flat amount, by adding to the provided parameter 
		/// </summary>
		/// <param name="additive">The current stat value, before applying stat scaling</param>
		public delegate void ResolveFlat_delegate(ref float additive);

		/// <inheritdoc cref="ResolveFlat_delegate"/>
		public static event ResolveFlat_delegate? ResolveFlat;

		internal static void ResolveFlat_Invoke(ref float additive) {
			ResolveFlat?.Invoke(ref additive);
		}

		/// <summary>
		/// Used to change Resolve scaling, by modifying the provided parameter
		/// </summary>
		/// <param name="multiplier">The current stat scaling value</param>
		public delegate void ResolveMultiplier_delegate(ref float multiplier);

		/// <inheritdoc cref="ResolveMultiplier_delegate"/>
		public static event ResolveMultiplier_delegate? ResolveMultiplier;

		internal static void ResolveMultiplier_Invoke(ref float multiplier) {
			ResolveMultiplier?.Invoke(ref multiplier);
		}
		#endregion

		#region Dexterity
		/// <summary>
		/// Used to change Dexterity by a flat amount, by adding to the provided parameter 
		/// </summary>
		/// <param name="additive">The current stat value, before applying stat scaling</param>
		public delegate void DexterityFlat_delegate(ref float additive);

		/// <inheritdoc cref="DexterityFlat_delegate"/>
		public static event DexterityFlat_delegate? DexterityFlat;

		internal static void DexterityFlat_Invoke(ref float additive) {
			DexterityFlat?.Invoke(ref additive);
		}

		/// <summary>
		/// Used to change Dexterity scaling, by modifying the provided parameter
		/// </summary>
		/// <param name="multiplier">The current stat scaling value</param>
		public delegate void DexterityMultiplier_delegate(ref float multiplier);

		/// <inheritdoc cref="DexterityMultiplier_delegate"/>
		public static event DexterityMultiplier_delegate? DexterityMultiplier;

		internal static void DexterityMultiplier_Invoke(ref float multiplier) {
			DexterityMultiplier?.Invoke(ref multiplier);
		}
		#endregion

		#region CritChance
		/// <summary>
		/// Used to change CritChance by a flat amount, by adding to the provided parameter 
		/// </summary>
		/// <param name="additive">The current stat value, before applying stat scaling</param>
		public delegate void CritChance_delegate(ref float additive);

		/// <inheritdoc cref="CritChance_delegate"/>
		public static event CritChance_delegate? CritChance;

		internal static void CritChance_Invoke(ref float additive) {
			CritChance?.Invoke(ref additive);
		}
		#endregion

		#region CritDamage
		/// <summary>
		/// Used to change CritDamage by a flat amount, by adding to the provided parameter 
		/// </summary>
		/// <param name="additive">The current stat value, before applying stat scaling</param>
		public delegate void CritDamage_delegate(ref float additive);

		/// <inheritdoc cref="CritDamage_delegate"/>
		public static event CritDamage_delegate? CritDamage;

		internal static void CritDamage_Invoke(ref float additive) {
			CritDamage?.Invoke(ref additive);
		}
		#endregion

		#region Focus
		/// <summary>
		/// Used to change Focus by a flat amount, by adding to the provided parameter 
		/// </summary>
		/// <param name="additive">The current stat value, before applying stat scaling</param>
		public delegate void FocusFlat_delegate(ref float additive);

		/// <inheritdoc cref="FocusFlat_delegate"/>
		public static event FocusFlat_delegate? FocusFlat;

		internal static void FocusFlat_Invoke(ref float additive) {
			FocusFlat?.Invoke(ref additive);
		}

		/// <summary>
		/// Used to change Focus scaling, by modifying the provided parameter
		/// </summary>
		/// <param name="multiplier">The current stat scaling value</param>
		public delegate void FocusMultiplier_delegate(ref float multiplier);

		/// <inheritdoc cref="FocusMultiplier_delegate"/>
		public static event FocusMultiplier_delegate? FocusMultiplier;

		internal static void FocusMultiplier_Invoke(ref float multiplier) {
			FocusMultiplier?.Invoke(ref multiplier);
		}
		#endregion

		#region MagicCritChance
		/// <summary>
		/// Used to change MagicCritChance by a flat amount, by adding to the provided parameter 
		/// </summary>
		/// <param name="additive">The current stat value, before applying stat scaling</param>
		public delegate void MagicCritChance_delegate(ref float additive);

		/// <inheritdoc cref="MagicCritChance_delegate"/>
		public static event MagicCritChance_delegate? MagicCritChance;

		internal static void MagicCritChance_Invoke(ref float additive) {
			MagicCritChance?.Invoke(ref additive);
		}
		#endregion

		#region MagicCritDamage
		/// <summary>
		/// Used to change MagicCritDamage by a flat amount, by adding to the provided parameter 
		/// </summary>
		/// <param name="additive">The current stat value, before applying stat scaling</param>
		public delegate void MagicCritDamage_delegate(ref float additive);

		/// <inheritdoc cref="MagicCritDamage_delegate"/>
		public static event MagicCritDamage_delegate? MagicCritDamage;

		internal static void MagicCritDamage_Invoke(ref float additive) {
			MagicCritDamage?.Invoke(ref additive);
		}
		#endregion

		#region Armor
		/// <summary>
		/// Used to change Armor by a flat amount, by adding to the provided parameter 
		/// </summary>
		/// <param name="additive">The current stat value, before applying stat scaling</param>
		public delegate void ArmorFlat_delegate(ref int additive);

		/// <inheritdoc cref="ArmorFlat_delegate"/>
		public static event ArmorFlat_delegate? ArmorFlat;

		internal static void ArmorFlat_Invoke(ref int additive) {
			ArmorFlat?.Invoke(ref additive);
		}

		/// <summary>
		/// Used to change Armor scaling, by modifying the provided parameter
		/// </summary>
		/// <param name="multiplier">The current stat scaling value</param>
		public delegate void ArmorMultiplier_delegate(ref float multiplier);

		/// <inheritdoc cref="ArmorMultiplier_delegate"/>
		public static event ArmorMultiplier_delegate? ArmorMultiplier;

		internal static void ArmorMultiplier_Invoke(ref float multiplier) {
			ArmorMultiplier?.Invoke(ref multiplier);
		}
		#endregion

		#region Strength
		/// <summary>
		/// Used to change Strength by a flat amount, by adding to the provided parameter 
		/// </summary>
		/// <param name="additive">The current stat value, before applying stat scaling</param>
		public delegate void StrengthFlat_delegate(ref float additive);

		/// <inheritdoc cref="StrengthFlat_delegate"/>
		public static event StrengthFlat_delegate? StrengthFlat;

		internal static void StrengthFlat_Invoke(ref float additive) {
			StrengthFlat?.Invoke(ref additive);
		}

		/// <summary>
		/// Used to change Strength scaling, by modifying the provided parameter
		/// </summary>
		/// <param name="multiplier">The current stat scaling value</param>
		public delegate void StrengthMultiplier_delegate(ref float multiplier);

		/// <inheritdoc cref="StrengthMultiplier_delegate"/>
		public static event StrengthMultiplier_delegate? StrengthMultiplier;

		internal static void StrengthMultiplier_Invoke(ref float multiplier) {
			StrengthMultiplier?.Invoke(ref multiplier);
		}
		#endregion

		#region Intelligence
		/// <summary>
		/// Used to change Intelligence by a flat amount, by adding to the provided parameter 
		/// </summary>
		/// <param name="additive">The current stat value, before applying stat scaling</param>
		public delegate void IntelligenceFlat_delegate(ref float additive);

		/// <inheritdoc cref="IntelligenceFlat_delegate"/>
		public static event IntelligenceFlat_delegate? IntelligenceFlat;

		internal static void IntelligenceFlat_Invoke(ref float additive) {
			IntelligenceFlat?.Invoke(ref additive);
		}

		/// <summary>
		/// Used to change Intelligence scaling, by modifying the provided parameter
		/// </summary>
		/// <param name="multiplier">The current stat scaling value</param>
		public delegate void IntelligenceMultiplier_delegate(ref float multiplier);

		/// <inheritdoc cref="IntelligenceMultiplier_delegate"/>
		public static event IntelligenceMultiplier_delegate? IntelligenceMultiplier;

		internal static void IntelligenceMultiplier_Invoke(ref float multiplier) {
			IntelligenceMultiplier?.Invoke(ref multiplier);
		}
		#endregion

		#region Vitality
		/// <summary>
		/// Used to change Vitality by a flat amount, by adding to the provided parameter 
		/// </summary>
		/// <param name="additive">The current stat value, before applying stat scaling</param>
		public delegate void VitalityFlat_delegate(ref int additive);

		/// <inheritdoc cref="VitalityFlat_delegate"/>
		public static event VitalityFlat_delegate? VitalityFlat;

		internal static void VitalityFlat_Invoke(ref int additive) {
			VitalityFlat?.Invoke(ref additive);
		}

		/// <summary>
		/// Used to change Vitality scaling, by modifying the provided parameter
		/// </summary>
		/// <param name="multiplier">The current stat scaling value</param>
		public delegate void VitalityMultiplier_delegate(ref float multiplier);

		/// <inheritdoc cref="VitalityMultiplier_delegate"/>
		public static event VitalityMultiplier_delegate? VitalityMultiplier;

		internal static void VitalityMultiplier_Invoke(ref float multiplier) {
			VitalityMultiplier?.Invoke(ref multiplier);
		}
		#endregion

		#region MaxHealth
		/// <summary>
		/// Used to change max Health by a flat amount, by adding to the provided parameter 
		/// </summary>
		/// <param name="additive">The current stat value, before applying stat scaling</param>
		public delegate void MaxHealthFlat_delegate(ref int additive);

		/// <inheritdoc cref="MaxHealthFlat_delegate"/>
		public static event MaxHealthFlat_delegate? MaxHealthFlat;

		internal static void MaxHealthFlat_Invoke(ref int additive) {
			MaxHealthFlat?.Invoke(ref additive);
		}

		/// <summary>
		/// Used to change MaxHealth scaling, by modifying the provided parameter
		/// </summary>
		/// <param name="multiplier">The current stat scaling value</param>
		public delegate void MaxHealthMultiplier_delegate(ref float multiplier);

		/// <inheritdoc cref="MaxHealthMultiplier_delegate"/>
		public static event MaxHealthMultiplier_delegate? MaxHealthMultiplier;

		internal static void MaxHealthMultiplier_Invoke(ref float multiplier) {
			MaxHealthMultiplier?.Invoke(ref multiplier);
		}
		#endregion

		#region MaxMana
		/// <summary>
		/// Used to change max Mana by a flat amount, by adding to the provided parameter 
		/// </summary>
		/// <param name="additive">The current stat value, before applying stat scaling</param>
		public delegate void MaxManaFlat_delegate(ref int additive);

		/// <inheritdoc cref="MaxManaFlat_delegate"/>
		public static event MaxManaFlat_delegate? MaxManaFlat;

		internal static void MaxManaFlat_Invoke(ref int additive) {
			MaxManaFlat?.Invoke(ref additive);
		}

		/// <summary>
		/// Used to change MaxMana scaling, by modifying the provided parameter
		/// </summary>
		/// <param name="multiplier">The current stat scaling value</param>
		public delegate void MaxManaMultiplier_delegate(ref float multiplier);

		/// <inheritdoc cref="MaxManaMultiplier_delegate"/>
		public static event MaxManaMultiplier_delegate? MaxManaMultiplier;

		internal static void MaxManaMultiplier_Invoke(ref float multiplier) {
			MaxManaMultiplier?.Invoke(ref multiplier);
		}
		#endregion

		#region RuneWeight
		/// <summary>
		/// Used to change Rune weight by a flat amount, by adding to the provided parameter 
		/// </summary>
		/// <param name="additive">The current stat value, before applying stat scaling</param>
		public delegate void RuneWeight_delegate(ref int additive);

		/// <inheritdoc cref="RuneWeight_delegate"/>
		public static event RuneWeight_delegate? RuneWeight;

		internal static void RuneWeight_Invoke(ref int additive) {
			RuneWeight?.Invoke(ref additive);
		}
		#endregion

		#region EquipmentWeight
		/// <summary>
		/// Used to change Equipment weight by a flat amount, by adding to the provided parameter 
		/// </summary>
		/// <param name="additive">The current stat value, before applying stat scaling</param>
		public delegate void EquipmentWeight_delegate(ref int additive);

		/// <inheritdoc cref="EquipmentWeight_delegate"/>
		public static event EquipmentWeight_delegate? EquipmentWeight;

		internal static void EquipmentWeight_Invoke(ref int additive) {
			EquipmentWeight?.Invoke(ref additive);
		}
		#endregion
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
		/// <param name="characterData">The randomized heirs CharacterData</param>
		public delegate void ModifyCharacterRandomization_delegate(CharacterData data);

		/// <inheritdoc cref="ModifyCharacterRandomization_delegate"/>
		public static event ModifyCharacterRandomization_delegate? ModifyCharacterRandomization;
		
		internal static void ModifyCharacterRandomization_Invoke(CharacterData data) {
			ModifyCharacterRandomization?.Invoke(data);
		}

		/// <summary>
		/// Used to modify character data of generated heirs
		/// </summary>
		/// <param name="characterData">The generated heirs <see cref="CharacterData" /></param>
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
		/// <param name="characterData">The generated heirs <see cref="CharacterData" /></param>
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