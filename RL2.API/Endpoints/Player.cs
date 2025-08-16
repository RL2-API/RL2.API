using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MoreMountains.CorgiEngine;
using System;
using System.Reflection;
using UnityEngine;

namespace RL2.API;

/// <summary>
/// Provides endpoints for player related APIs
/// </summary>
public static partial class Player
{
	internal static IDetour[] Hooks = [
		OnSpawn.Hook,
		PreKill.Hook,
		OnKill.Hook,
		PostUpdateStats.Hook,
		..HeirGeneration.Hooks,
		..Ability.Hooks,
	];

	/// <inheritdoc cref="Definition"/>
	public static class OnSpawn
	{
		/// <summary>
		/// Ran on player spawn
		/// </summary>
		/// <param name="player">The instance of the player that was spawned</param>
		public delegate void Definition(PlayerController player);

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static Hook Hook = new Hook(
			typeof(PlayerController).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance),
			Method,
			new HookConfig() {
				ID = "RL2.API::PLayer.OnSpawn",
				ManualApply = true,
			}
		);

		internal static void Method(Action<PlayerController> orig, PlayerController self) {
			orig(self);
			Event?.Invoke(self);
		}
	}

	/// <inheritdoc cref="Definition"/>
	public static class PreKill
	{
		/// <summary>
		/// Determines whether the affected player should die.
		/// </summary>
		/// <param name="player">Affected player</param>
		/// <param name="killer">GameObject responsible for the player's death</param>
		/// <returns>If the player should die - <c>false</c> to prevent death</returns>
		public delegate bool Definition(PlayerController player, GameObject killer);

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static Hook Hook = new Hook(
			typeof(PlayerController).GetMethod("KillCharacter", BindingFlags.Public | BindingFlags.Instance),
			Method,
			new HookConfig() {
				ID = "RL2.API::PLayer.PreKill",
				ManualApply = true,
			}
		);

		internal static void Method(Action<PlayerController, GameObject, bool> orig, PlayerController self, GameObject killer, bool broadcastEvents) {
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

	/// <inheritdoc cref="Definition"/>
	public static class OnKill
	{
		/// <summary>
		/// Ran after the player dies
		/// </summary>
		/// <param name="player">Affected player</param>
		/// <param name="killer">GameObject responsible for the player's death</param>
		public delegate void Definition(PlayerController player, GameObject killer);

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static Hook Hook = new Hook(
			typeof(PlayerController).GetMethod("KillCharacter", BindingFlags.Public | BindingFlags.Instance),
			Method,
			new HookConfig() {
				ID = "RL2.API::Player.OnKill",
				ManualApply = true,
			}
		);

		internal static void Method(Action<PlayerController, GameObject, bool> orig, PlayerController self, GameObject killer, bool broadcastEvents) {
			orig(self, killer, broadcastEvents);
			Event?.Invoke(self, killer);
		}
	}

	/// <inheritdoc cref="Definition"/>
	public static class PostUpdateStats
	{
		/// <summary>
		/// Executed after all stats update. Use to perform stat increases.
		/// </summary>
		/// <param name="player">The player</param>
		public delegate void Definition(PlayerController player);

		/// <inheritdoc cref="Definition" />
		public static event Definition? Event;

		internal static Hook Hook = new Hook(
			typeof(PlayerController).GetMethod("InitializeAllMods", BindingFlags.Public | BindingFlags.Instance),
			Method,
			new HookConfig() {
				ID = "RL2.API::Player.PostUpdateStats",
				ManualApply = true,
			}
		);

		internal static void Method(Action<PlayerController, bool, bool> orig, PlayerController self, bool resetHP, bool resetMP) {
			orig(self, resetHP, resetMP);
			Event?.Invoke(self);
		}

	}
}