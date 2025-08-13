using RL2.API;
using RL2.API.SummonRules;
using System.Collections.Generic;

namespace ExampleMod;

public class AlterScar : IRegistrable
{
	void IRegistrable.Register() {
		Scars.ModifySummonRules += AlterArmada;
	}

	internal Scars.ModifySummonRules_delegate AlterArmada = (ChallengeType challenge, ref List<BaseSummonRule> rules) => {
		if (challenge == ChallengeType.BigBattle) {
			var count = rules.Count - 1;
			for (var i = count - 10; i > 6; i--) {
				rules.RemoveAt(i);
			}

			List<BaseSummonRule> newRules = [
				new SetEnemyLevel() {
					Level = 12,
					SetToRoomLevel = false
				},

				new SetEnemyPool() {
					EnemyPool = [new(EnemyType.SpellswordBoss, EnemyRank.Basic)],
					IsBiomeSpecific = false,
					FlyingOnly = false
				},
				new SummonEnemies() { SummonValue = 1, SummonDelay = 0, RandomizeOnce = true, SpawnFast = false, SpawnAsCommander = false },

				new SetEnemyPool() {
					EnemyPool = [new(EnemyType.SkeletonBossA, EnemyRank.Basic)],
					IsBiomeSpecific = false,
					FlyingOnly = false
				},
				new SummonEnemies() { SummonValue = 1, SummonDelay = 0, RandomizeOnce = true, SpawnFast = false, SpawnAsCommander = false },

				new SetEnemyPool() {
					EnemyPool = [new(EnemyType.DancingBoss, EnemyRank.Basic)],
					IsBiomeSpecific = false,
					FlyingOnly = false
				},
				new SummonEnemies() { SummonValue = 1, SummonDelay = 0, RandomizeOnce = true, SpawnFast = false, SpawnAsCommander = false },

				new SetEnemyPool() {
					EnemyPool = [new(EnemyType.StudyBoss, EnemyRank.Basic)],
					IsBiomeSpecific = false,
					FlyingOnly = false
				},
				new SummonEnemies() { SummonValue = 1, SummonDelay = 0, RandomizeOnce = true, SpawnFast = false, SpawnAsCommander = false },

				new SetEnemyPool() {
					EnemyPool = [new(EnemyType.EyeballBoss_Bottom, EnemyRank.Basic)],
					IsBiomeSpecific = false,
					FlyingOnly = false
				},
				new SummonEnemies() { SummonValue = 1, SummonDelay = 0, RandomizeOnce = true, SpawnFast = false, SpawnAsCommander = false },

				new SetEnemyPool() {
					EnemyPool = [new(EnemyType.CaveBoss, EnemyRank.Basic)],
					IsBiomeSpecific = false,
					FlyingOnly = false
				},
				new SummonEnemies() { SummonValue = 1, SummonDelay = 0, RandomizeOnce = true, SpawnFast = false, SpawnAsCommander = false },
			];

			rules.InsertRange(6, newRules);
		}
	};
}