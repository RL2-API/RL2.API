using MonoMod.RuntimeDetour;
using RL2.API.DataStructures;
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
		ModifyDamageTaken.Hook,
		ModifyData.Hook,
		ModifyBehaviour.Hook
	];

	/// <inheritdoc cref="Definition" />
	public static class OnSpawn
	{
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
	public static class PreKill
	{
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
	public static class OnKill
	{
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

	/// <inheritdoc cref="Definition" />
	public static class ModifyDamageTaken
	{
		/// <summary>
		///	Allows modders to modify the damage taken by enemies
		/// </summary>
		/// <param name="enemyDamaged">The enemy damaged</param>
		/// <param name="damageSource">Object that damaged the enemy</param>
		/// <param name="damageTaken">How much damage the enemy received</param>
		/// <param name="critType">Critical strike type</param>
		/// <param name="damageTakenModifiers">Allows for modifying the damage taken</param>
		public delegate void Definition(EnemyController enemyDamaged, IDamageObj damageSource, float damageTaken, ref CriticalStrikeType critType, ref Modifiers damageTakenModifiers);

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static Hook Hook = new Hook(
			typeof(EnemyController).GetMethod("CalculateDamageTaken", BindingFlags.Public | BindingFlags.Instance),
			Method,
			new HookConfig() {
				ID = "RL2.API::Enemy.ModifyDamageTaken",
				ManualApply = true,
			}
		);

		internal delegate float OriginalDefinition(EnemyController self, IDamageObj damageObj, out CriticalStrikeType critType, out float damageBlocked, out float dmgBlockedByMana, float damageOverride, bool trueDamage, bool pureCalculation);

		internal static float Method(OriginalDefinition orig, EnemyController self, IDamageObj damageObj, out CriticalStrikeType critType, out float damageBlocked, out float dmgBlockedByMana, float damageOverride, bool trueDamage, bool pureCaclulation) {
			float original = orig(self, damageObj, out critType, out damageBlocked, out dmgBlockedByMana, damageOverride, trueDamage, pureCaclulation);
			if (!trueDamage) {
				Modifiers modifiers = new Modifiers();
				Event?.Invoke(self, damageObj, original, ref critType, ref modifiers);
				original += modifiers.Additive;
				original *= modifiers.Multiplicative;
				original += modifiers.Flat;
			}
			return original;
		}

	}

	/// <inheritdoc cref="Definition" />
	public static class ModifyData
	{
		/// <summary>
		/// Allows for modifying <see cref="EnemyData"/>
		/// </summary>
		/// <param name="type">Enemy type</param>
		/// <param name="rank">Enemy rank</param>
		/// <param name="data">Enemy data</param>
		public delegate void Definition(EnemyType type, EnemyRank rank, EnemyData data);

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static Hook Hook = new Hook(
			typeof(EnemyTypeEnemyClassDataDictionary).GetMethod("TryGetValue", BindingFlags.Public | BindingFlags.Instance),
			Method,
			new HookConfig() {
				ID = "RL2.API::Enemy.ModifyData",
				ManualApply = true,
				Before = ["RL2.API::Enemy.ModifyBehaviour"]
			}
		);

		internal delegate bool OriginalDefinition(EnemyTypeEnemyClassDataDictionary self, EnemyType key, out EnemyClassData data);

		internal static bool Method(OriginalDefinition orig, EnemyTypeEnemyClassDataDictionary self, EnemyType type, out EnemyClassData data) {
			bool found = orig(self, type, out data);
			if (found) {
				foreach (EnemyRank rank in Enum.GetValues(typeof(EnemyRank)))
					Event?.Invoke(type, rank, data.GetEnemyData(rank));
			}
			return found;
		}
	}

	/// <inheritdoc cref="Definition" />
	public static class ModifyBehaviour
	{
		/// <summary>
		/// Allows for modifying the enemies behaviour.<br></br>
		/// Warning: When setting <paramref name="aiScript"/> to a new instance of a adifferent type, one must also replace <paramref name="logicController_SO"/> with the correctly built <see cref="LogicController_SO"/> object
		/// </summary>
		/// <param name="type">Enemy type</param>
		/// <param name="rank">Enemy rank</param>
		/// <param name="aiScript">Enemy's AI script object</param>
		/// <param name="logicController_SO">Enemy's logic controller scriptable object. Handles chances of behaviour</param>
		public delegate void Definition(EnemyType type, EnemyRank rank, ref BaseAIScript aiScript, ref LogicController_SO logicController_SO);

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static Hook Hook = new Hook(
			typeof(EnemyTypeEnemyClassDataDictionary).GetMethod("TryGetValue", BindingFlags.Public | BindingFlags.Instance),
			Method,
			new HookConfig() {
				ID = "RL2.API::Enemy.ModifyData",
				ManualApply = true,
			}
		);

		internal delegate bool OriginalDefinition(EnemyTypeEnemyClassDataDictionary self, EnemyType key, out EnemyClassData data);

		internal static bool Method(OriginalDefinition orig, EnemyTypeEnemyClassDataDictionary self, EnemyType type, out EnemyClassData data) {
			bool found = orig(self, type, out data);
			if (found) {
				Event?.Invoke(type, EnemyRank.Basic, ref data.m_basicAIScript, ref data.m_enemyLogicController);
				Event?.Invoke(type, EnemyRank.Advanced, ref data.m_advancedAIScript, ref data.m_enemyLogicController);
				Event?.Invoke(type, EnemyRank.Expert, ref data.m_expertAIScript, ref data.m_enemyLogicController);
				Event?.Invoke(type, EnemyRank.Miniboss, ref data.m_minibossAIScript, ref data.m_enemyLogicController);
			}
			return found;
		}
	}
}