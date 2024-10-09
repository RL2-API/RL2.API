using System;
using UnityEngine;

namespace RL2.ModLoader.API;

/// <summary>
/// Provides endpoints for enemy related APIs
/// </summary>
public class Enemy
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
			return false;
		}

		bool result = false;
		foreach (Delegate subscriber in PreKill.GetInvocationList()) {
			result |= (bool)subscriber.DynamicInvoke(enemy, killer);
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
}