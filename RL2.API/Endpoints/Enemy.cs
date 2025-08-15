using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace RL2.API;

/// <summary>
/// Provides endpoints for enemy related APIs
/// </summary>
public static class Enemy
{
	internal static IDetour[] Hooks = [
		OnSpawn.Hook,
		PreKill.Hook,
		OnKill.Hook,
	];

	/// <inheritdoc cref="Definition" />
	public static class OnSpawn {
		/// <summary>
		/// Ran on enemy spawn
		/// </summary>
		/// <param name="enemy">The instance of the enemy that was spawned</param>
		public delegate void Definition(EnemyController enemy);

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static Hook Hook = new Hook(
			typeof(EnemyController).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance),
			Method,
			new HookConfig() {
				ID = "RL2.API::Enemy.OnSpawn",
				ManualApply = true,
			}
		);

		internal static IEnumerator Method(Func<EnemyController, IEnumerator> orig, EnemyController self) {
			IEnumerator originalStart = orig(self);

			while (originalStart.MoveNext()) {
				yield return originalStart.Current;
			}

			Event?.Invoke(self);
		}
	}

	/// <inheritdoc cref="Definition" />
	public static class PreKill {
		/// <summary>
		/// Determines whether the affected enemy should die.
		/// </summary>
		/// <param name="enemy">Affected enemy</param>
		/// <param name="killer">GameObject responsible for the enemy's death</param>
		/// <returns>If the enemy should die</returns>
		public delegate bool Definition(EnemyController enemy, GameObject killer);

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static Hook Hook = new Hook(
			typeof(EnemyController).GetMethod("KillCharacter", BindingFlags.Public | BindingFlags.Instance),
			Method,
			new HookConfig() {
				ID = "RL2.API::Enemy.PreKill",
				ManualApply = true,
			}
		);

		internal static void Method(Action<EnemyController, GameObject, bool> orig, EnemyController self, GameObject killer, bool broadcastEvents) {
			bool shouldDie = true;
			if (self.IsDead) {
				orig(self, killer, broadcastEvents);
				return;
			}

			foreach (Delegate subscriber in Event?.GetInvocationList() ?? [])
				shouldDie &= (bool)subscriber.DynamicInvoke(self, killer);

			if (shouldDie) {
				orig(self, killer, broadcastEvents);
				return;
			}
			self.SetHealth(1f, additive: false, runEvents: true);
		}
	}

	/// <inheritdoc cref="Definition" />
	public static class OnKill {
		/// <summary>
		/// Ran after the enemy dies
		/// </summary>
		/// <param name="enemy">Affected enemy</param>
		/// <param name="killer">GameObject responsible for the enemy's death</param>
		public delegate void Definition(EnemyController enemy, GameObject killer);

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static Hook Hook = new Hook(
			typeof(EnemyController).GetMethod("KillCharacter", BindingFlags.Public | BindingFlags.Instance),
			Method,
			new HookConfig() {
				ID = "RL2.API::Enemy.OnKill",
				ManualApply = true,
			}
		);

		internal static void Method(Action<EnemyController, GameObject, bool> orig, EnemyController self, GameObject killer, bool broadcastEvents) {
			orig(self, killer, broadcastEvents);
			Event?.Invoke(self, killer);
		}
	}
}
