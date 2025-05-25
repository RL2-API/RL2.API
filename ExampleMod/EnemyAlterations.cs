using RL2.API;

namespace ExampleMod;

public class EnemyAlterations : IRegistrable
{
	public void Register() {
		// Every enemy can spawn in the Castle
		Enemy.ModifyData += (EnemyType type, EnemyRank rank, EnemyData data) => {
			data.SpawnInCastle = true;
		};

		Enemy.ModifyBehaviour += (EnemyType type, EnemyRank rank, ref BaseAIScript aiScript, ref LogicController_SO logicController) => {
			// Apply effect only to basic Lamech
			if (type != EnemyType.SpellswordBoss || rank != EnemyRank.Basic) return;

			aiScript = EnemyClassLibrary.GetEnemyClassData(EnemyType.StudyBoss).GetAIScript(rank);
			logicController = EnemyClassLibrary.GetEnemyClassData(EnemyType.StudyBoss).GetLogicController();
		};
	}
}