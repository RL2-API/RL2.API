using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace RL2.ModLoader.APIEndpoints;

/// <summary>
/// Stores all RL2.API endpoints for the GlobalEnemy class
/// </summary>
public class GlobalEnemyEndpoints
{
	/// <summary>
	/// Handles attaching GlobalEnemy objects to the enemies and running OnSpawn
	/// </summary>
	public static Hook AttachGlobalEnemyInstnces = new Hook(
		typeof(EnemyController).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance),
		AttachGlobalEnemyInstance
	);

	/// <summary>
	/// <inheritdoc cref="AttachGlobalEnemyInstnces"/>
	/// </summary>
	public static IEnumerator AttachGlobalEnemyInstance(Func<EnemyController, IEnumerator> orig, EnemyController self) {
		yield return orig(self);
		foreach (Mod mod in RL2API.LoadedMods) {
			foreach (Type globalEnemy in mod.GetModTypes<GlobalEnemy>()) {
				GlobalEnemy globalEnemyInstance = (GlobalEnemy)self.gameObject.AddComponent(globalEnemy);
				if (globalEnemyInstance.AppliesToEnemy((int)self.EnemyType, self.EnemyRank)) {
						globalEnemyInstance.OnSpawn();
						continue;
				}

				UnityEngine.Object.Destroy(globalEnemyInstance);
			}
		}
	}

	/// <summary>
	/// Handles death prevention for enemies
	/// </summary>
	public static Hook PreKill = new Hook(
		typeof(EnemyController).GetMethod("KillCharacter", BindingFlags.Public | BindingFlags.Instance),
		(Action<EnemyController, GameObject, bool> orig, EnemyController self, GameObject killer, bool broadcastEvents) => {
			if (self.IsDead) {
				return;
			}

			bool PreventDeath = false;
			foreach (GlobalEnemy globalEnemy in self.gameObject.GetComponents<GlobalEnemy>()) {
				if (!globalEnemy.PreKill(killer)) {
					PreventDeath = true;
					break;
				}
			}
			if (PreventDeath) {
				self.SetHealth(1f, additive: false, runEvents: true);
				return;
			}
			orig(self, killer, broadcastEvents);
		}
	);

	/// <summary>
	/// Handles calling <seealso cref="GlobalEnemy.OnKill"/>
	/// </summary>
	public static Hook OnKill = new Hook(
		typeof(EnemyController).GetMethod("KillCharacter", BindingFlags.Public | BindingFlags.Instance),
		(Action<EnemyController, GameObject, bool> orig, EnemyController self, GameObject killer, bool broadcastEvents) => {
			orig(self, killer, broadcastEvents);
			foreach (GlobalEnemy globalEnemy in self.gameObject.GetComponents<GlobalEnemy>()) {
				globalEnemy.OnKill(killer);
			}
		}
	);
}