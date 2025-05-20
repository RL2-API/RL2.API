using RL2.API;
using System.Collections.Generic;

namespace ExampleMod;

public class AlterScar : IRegistrable {
	void IRegistrable.Register() {
		Scars.ModifySummonRules += AlterArmada;
	}

	internal Scars.ModifyScarSummonRules_delegate AlterArmada = (ChallengeType challenge, ref List<BaseSummonRule> rules) => {
		if (challenge == ChallengeType.BigBattle) {
			var count = rules.Count - 1;
			for (var i = count - 10; i > 6; i--) {
				rules.RemoveAt(i);
			}

			List<BaseSummonRule> newRules = [
				new SetSummonPoolLevelMod_SummonRule().Set(level: 12, setToPlayerLevel: false),
				
				new SetSummonPool_SummonRule().Set(
					enemyPool: [new(EnemyType.SpellswordBoss, EnemyRank.Basic)],
					isBiomeSpecific: false,
					flyingOnly: false
				),
				new SummonEnemy_SummonRule().Set(summonValue: 1, summonDelay: 0, randomizeOnce: true, spawnFast: false, spawnAsCommander: false),

				new SetSummonPool_SummonRule().Set(
					enemyPool: [new(EnemyType.SkeletonBossA, EnemyRank.Basic)],
					isBiomeSpecific: false,
					flyingOnly: false
				),
				new SummonEnemy_SummonRule().Set(summonValue: 1, summonDelay: 0, randomizeOnce: true, spawnFast: false, spawnAsCommander: false),

				new SetSummonPool_SummonRule().Set(
					enemyPool: [new(EnemyType.DancingBoss, EnemyRank.Basic)],
					isBiomeSpecific: false,
					flyingOnly: false
				),
				new SummonEnemy_SummonRule().Set(summonValue: 1, summonDelay: 0, randomizeOnce: true, spawnFast: false, spawnAsCommander: false),

				new SetSummonPool_SummonRule().Set(
					enemyPool: [new(EnemyType.StudyBoss, EnemyRank.Basic)],
					isBiomeSpecific: false,
					flyingOnly: false
				),
				new SummonEnemy_SummonRule().Set(summonValue: 1, summonDelay: 0, randomizeOnce: true, spawnFast: false, spawnAsCommander: false),

				new SetSummonPool_SummonRule().Set(
					enemyPool: [new(EnemyType.EyeballBoss_Middle, EnemyRank.Basic)],
					isBiomeSpecific: false,
					flyingOnly: false
				),
				new SummonEnemy_SummonRule().Set(summonValue: 1, summonDelay: 0, randomizeOnce: true, spawnFast: false, spawnAsCommander: false),

				new SetSummonPool_SummonRule().Set(
					enemyPool: [new(EnemyType.CaveBoss, EnemyRank.Basic)],
					isBiomeSpecific: false,
					flyingOnly: false
				),
				new SummonEnemy_SummonRule().Set(summonValue: 1, summonDelay: 0, randomizeOnce: true, spawnFast: false, spawnAsCommander: false),

				new SetSummonPool_SummonRule().Set(
					enemyPool: [new(EnemyType.FinalBoss, EnemyRank.Basic)],
					isBiomeSpecific: false,
					flyingOnly: false
				),
				new SummonEnemy_SummonRule().Set(summonValue: 1, summonDelay: 0, randomizeOnce: true, spawnFast: false, spawnAsCommander: false),
			];

			rules.InsertRange(6, newRules);
		}
	};
}