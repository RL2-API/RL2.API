using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace RL2.ModLoader;

/// <summary>
/// 
/// </summary>
public static class GlobalEnemyEndpoints {
	/// <summary>
	/// 
	/// </summary>
	public static Hook AttachGlobalEnemyInstnces = new Hook(
		typeof(EnemyController).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance),
		(Func<EnemyController, IEnumerator> orig, EnemyController self) => {
			orig(self);
			foreach (Mod mod in APIStore.LoadedMods) {
				foreach (Type globalEnemy in mod.GetModTypes<GlobalEnemy>()) {
					GlobalEnemy globalEnemyInstance = (GlobalEnemy)self.gameObject.AddComponent(globalEnemy);

					// If the AppliesToEnemy dictionary is empty, assume that this GlobalEnemy should be applied to every enemy.
					if (globalEnemyInstance.AppliesToEnemy.Keys.Count == 0) {
						globalEnemyInstance.OnSpawn();
						continue;
					}

					if (globalEnemyInstance.AppliesToEnemy.TryGetValue((int)self.EnemyType, out EnemyRank[] enemyRanks)) {
						if (enemyRanks.IndexOf(self.EnemyRank) != -1) {
							globalEnemyInstance.OnSpawn();
							continue;
						}
					}

					UnityEngine.Object.Destroy(globalEnemyInstance);
				}
			}
		}
	);

	/// <summary>
	/// 
	/// </summary>
	public static Hook PreAndOnKill = new Hook(
		typeof(EnemyController).GetMethod("KillCharacter", BindingFlags.NonPublic | BindingFlags.Instance),
		(Action<EnemyController, GameObject, bool> orig, EnemyController self, GameObject killer, bool broadcastEvents) => {
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
			foreach (GlobalEnemy globalEnemy in self.gameObject.GetComponents<GlobalEnemy>()) {
				globalEnemy.OnKill(killer);
			}
		}
	);
}