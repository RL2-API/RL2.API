using MonoMod.RuntimeDetour;
using RL2.ModLoader.API;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace RL2.ModLoader;

public partial class RL2API
{
	/// <summary>
	/// Handles calling <see cref="Enemy.OnSpawn"/>
	/// </summary>
	internal static Hook OnSpawn_Enemy = new Hook(
		typeof(EnemyController).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance),
		Run_Enemy_OnSpawn
	);

	/// <summary>
	/// <inheritdoc cref="OnSpawn_Enemy"/>
	/// </summary>
	internal static IEnumerator Run_Enemy_OnSpawn(Func<EnemyController, IEnumerator> orig, EnemyController self) {
		IEnumerator originalStart = orig(self);

		while (originalStart.MoveNext()) {
			yield return originalStart.Current;
		}

		Enemy.OnSpawn_Invoke(self);
	}

	/// <summary>
	/// Handles calling <see cref="Enemy.PreKill"/>
	/// </summary>
	internal static Hook PreKill_Enemy = new Hook(
		typeof(EnemyController).GetMethod("KillCharacter", BindingFlags.Public | BindingFlags.Instance),
		(Action<EnemyController, GameObject, bool> orig, EnemyController self, GameObject killer, bool broadcastEvents) => {
			if (self.IsDead) {
				return;
			}

			bool PreventDeath = Enemy.PreKill_Invoke(self, killer);
			if (PreventDeath) {
				self.SetHealth(1f, additive: false, runEvents: true);
				return;
			}

			orig(self, killer, broadcastEvents);
		}
	);

	/// <summary>
	/// Handles calling <seealso cref="Enemy.OnKill"/>
	/// </summary>
	internal static Hook OnKill_Enemy = new Hook(
		typeof(EnemyController).GetMethod("KillCharacter", BindingFlags.Public | BindingFlags.Instance),
		(Action<EnemyController, GameObject, bool> orig, EnemyController self, GameObject killer, bool broadcastEvents) => {
			orig(self, killer, broadcastEvents);
			Enemy.OnKill_Invoke(self, killer);
		}
	);
}