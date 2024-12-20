using MonoMod.RuntimeDetour;
using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace RL2.API;

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

	#region Death
	/// <summary>
	/// Handles calling <see cref="Enemy.PreKill"/>
	/// </summary>
	internal static Hook PreKill_Enemy = new Hook(
		typeof(EnemyController).GetMethod("KillCharacter", BindingFlags.Public | BindingFlags.Instance),
		(Action<EnemyController, GameObject, bool> orig, EnemyController self, GameObject killer, bool broadcastEvents) => {
			if (self.IsDead) {
				return;
			}

			bool PreventDeath = !Enemy.PreKill_Invoke(self, killer);
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
	#endregion

	internal static Hook Enemy_ModifyDamageTaken_Hook = new Hook(
		typeof(EnemyController).GetMethod("CalculateDamageTaken", BindingFlags.Public | BindingFlags.Instance),
		Enemy_ModifyDamageTaken_Method
	);

	internal delegate float Enemy_ModifyDamageTaken_delegate(EnemyController self, IDamageObj damageObj, out CriticalStrikeType critType, out float damageBlocked, out float dmgBlockedByMana, float damageOverride, bool trueDamage, bool pureCalculation);

	internal static float Enemy_ModifyDamageTaken_Method(Enemy_ModifyDamageTaken_delegate orig, EnemyController self, IDamageObj damageObj, out CriticalStrikeType critType, out float damageBlocked, out float dmgBlockedByMana, float damageOverride, bool trueDamage, bool pureCaclulation) {
		float original = orig(self, damageObj, out critType, out damageBlocked, out dmgBlockedByMana, damageOverride, trueDamage, pureCaclulation);
		Enemy.ModifyDamageTaken_Invoke(self, damageObj, ref original, ref critType, trueDamage);
		return original;
	}

	#region
	internal delegate bool EnemyClassDataDictionary_TryGetValue(EnemyTypeEnemyClassDataDictionary self, EnemyType key, out EnemyClassData data);

	internal static Hook ModifyClassDataHook = new Hook(
		typeof(EnemyTypeEnemyClassDataDictionary).GetMethod("TryGetValue", BindingFlags.Public | BindingFlags.Instance),
		ModifyClassDataMethod
	);

	internal static bool ModifyClassDataMethod(EnemyClassDataDictionary_TryGetValue orig, EnemyTypeEnemyClassDataDictionary self, EnemyType type, out EnemyClassData data) {
		bool found = orig(self, type, out data);
		if (found) {
			foreach (EnemyRank rank in Enum.GetValues(typeof(EnemyRank))) {
				Enemy.ModifyData_Invoke(type, rank, data.GetEnemyData(rank));
			}
			Enemy.ModifyBehaviour_Invoke(type, EnemyRank.Basic, ref data.m_basicAIScript, ref data.m_enemyLogicController);
			Enemy.ModifyBehaviour_Invoke(type, EnemyRank.Advanced, ref data.m_advancedAIScript, ref data.m_enemyLogicController);
			Enemy.ModifyBehaviour_Invoke(type, EnemyRank.Expert, ref data.m_expertAIScript, ref data.m_enemyLogicController);
			Enemy.ModifyBehaviour_Invoke(type, EnemyRank.Miniboss, ref data.m_minibossAIScript, ref data.m_enemyLogicController);
		}
		return found;
	}
	#endregion
}