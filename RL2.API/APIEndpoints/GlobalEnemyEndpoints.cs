using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace RL2.ModLoader;

/// <summary>
/// Stores all RL2.API endpoints for the GlobalEnemy class
/// </summary>
public partial class RL2API
{
	/// <summary>
	/// Handles attaching GlobalEnemy objects to the enemies and running OnSpawn
	/// </summary>
	internal static Hook AttachGlobalEnemyInstnces = new Hook(
		typeof(EnemyController).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance),
		AttachGlobalEnemyInstance
	);

	/// <summary>
	/// <inheritdoc cref="AttachGlobalEnemyInstnces"/>
	/// </summary>
	internal static IEnumerator AttachGlobalEnemyInstance(Func<EnemyController, IEnumerator> orig, EnemyController self) {
		IEnumerator originalStart = orig(self);

		while (originalStart.MoveNext()) {
			yield return originalStart.Current;
		}

		foreach (Mod mod in LoadedMods) {
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
	internal static Hook PreKill_Enemy = new Hook(
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
	internal static Hook OnKill_Enemy = new Hook(
		typeof(EnemyController).GetMethod("KillCharacter", BindingFlags.Public | BindingFlags.Instance),
		(Action<EnemyController, GameObject, bool> orig, EnemyController self, GameObject killer, bool broadcastEvents) => {
			orig(self, killer, broadcastEvents);
			foreach (GlobalEnemy globalEnemy in self.gameObject.GetComponents<GlobalEnemy>()) {
				globalEnemy.OnKill(killer);
			}
		}
	);
}