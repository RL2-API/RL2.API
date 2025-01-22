using System;
using UnityEngine;

namespace RL2.API;

/// <summary>
/// Provides endpoints for enemy related APIs
/// </summary>
public static class Enemy
{
	/// <summary>
	/// Ran on enemy spawn
	/// </summary>
	/// <param name="enemy">The instance of the enemy that was spawned</param>
	public delegate void OnSpawn_delegate(EnemyController enemy);

	/// <inheritdoc cref="OnSpawn_delegate"/>
	public static event OnSpawn_delegate? OnSpawn;

	internal static void OnSpawn_Invoke(EnemyController enemy) {
		OnSpawn?.Invoke(enemy);
	}

	#region Death
	/// <summary>
	/// Determines whether the affected enemy should die.
	/// </summary>
	/// <param name="enemy">Affected enemy</param>
	/// <param name="killer">GameObject responsible for the enemy's death</param>
	/// <returns>If the enemy should die</returns>
	public delegate bool PreKill_delegate(EnemyController enemy, GameObject killer);

	/// <inheritdoc cref="PreKill_delegate"/>
	public static event PreKill_delegate? PreKill;

	internal static bool PreKill_Invoke(EnemyController enemy, GameObject killer) {
		if (PreKill is null) {
			return true;
		}

		bool result = true;
		foreach (Delegate subscriber in PreKill.GetInvocationList()) {
			if (!result) {
				break;
			}

			result &= (bool)subscriber.DynamicInvoke(enemy, killer);
		}
		return result;
	}

	/// <summary>
	/// Ran after the enemy dies
	/// </summary>
	/// <param name="enemy">Affected enemy</param>
	/// <param name="killer">GameObject responsible for the enemy's death</param>
	public delegate void OnKill_delegate(EnemyController enemy, GameObject killer);

	/// <inheritdoc cref="OnKill_delegate"/>
	public static event OnKill_delegate? OnKill;

	internal static void OnKill_Invoke(EnemyController enemy, GameObject killer) {
		OnKill?.Invoke(enemy, killer);
	}
	#endregion

	#region Enemy prefab manipulation
	/// <summary>
	/// Allows for modifying <see cref="EnemyData"/>
	/// </summary>
	/// <param name="type">Enemy type</param>
	/// <param name="rank">Enemy rank</param>
	/// <param name="data">Enemy data</param>
	public delegate void ModifyData_delegate(EnemyType type, EnemyRank rank, EnemyData data);

	/// <inheritdoc cref="ModifyData_delegate"/>
	public static event ModifyData_delegate? ModifyData;

	internal static void ModifyData_Invoke(EnemyType type, EnemyRank rank, EnemyData data) {
		ModifyData?.Invoke(type, rank, data);
	}

	/// <summary>
	/// Allows for modifying the enemies behaviour.<br></br>
	/// Warning: When setting <paramref name="aiScript"/> to a new instance of a adifferent type, one must also replace <paramref name="logicController_SO"/> with the correctly built <see cref="LogicController_SO"/> object
	/// </summary>
	/// <param name="type">Enemy type</param>
	/// <param name="rank">Enemy rank</param>
	/// <param name="aiScript">Enemy's AI script object</param>
	/// <param name="logicController_SO">Enemy's logic controller scriptable object. Handles chances of behaviour</param>
	public delegate void ModifyBehaviour_delegate(EnemyType type, EnemyRank rank, ref BaseAIScript aiScript, ref LogicController_SO logicController_SO);

	/// <inheritdoc cref="ModifyBehaviour_delegate"/>
	public static event ModifyBehaviour_delegate? ModifyBehaviour;

	internal static void ModifyBehaviour_Invoke(EnemyType type, EnemyRank rank, ref BaseAIScript aiScript, ref LogicController_SO logicController_SO) {
		ModifyBehaviour?.Invoke(type, rank, ref aiScript, ref logicController_SO);
	}
	#endregion

	/// <summary>
	///	Allows modders to modify the damage taken by enemies
	/// </summary>
	/// <param name="enemyDamaged">The enemy damaged</param>
	/// <param name="damageSource">Object that damaged the enemy</param>
	/// <param name="damageTaken">How much damage the enemy received. Do not multiply, use <paramref name="damageMultiplier"/></param>
	/// <param name="damageMultiplier">Used for percentage based/multiplicative bonuses</param>
	/// <param name="critType">Critical strike type</param>
	public delegate void ModifyDamageTaken_delegate(EnemyController enemyDamaged, IDamageObj damageSource, ref float damageTaken, ref float damageMultiplier, ref CriticalStrikeType critType);

	/// <inheritdoc cref="ModifyDamageTaken_delegate"/>
	public static event ModifyDamageTaken_delegate? ModifyDamageTaken;

	internal static void ModifyDamageTaken_Invoke(EnemyController enemyDamaged, IDamageObj damageSource, ref float damageTaken, ref CriticalStrikeType critType, bool trueDamage) {
		if (!trueDamage) {
			float damageMultiplier = 1f;
			ModifyDamageTaken?.Invoke(enemyDamaged, damageSource, ref damageTaken, ref damageMultiplier, ref critType);
			damageTaken *= damageMultiplier;
		}
	}
}