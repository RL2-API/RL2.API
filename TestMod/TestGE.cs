using RL2.ModLoader;

namespace TestMod;

public class TestGE : GlobalEnemy
{
	public override EnemyType[] AppliesToEnemyType => new EnemyType[] { EnemyType.SpellswordBoss };

	public override void OnSpawn()
	{
		Enemy.LogicController.SwapAIScript<StudyBoss_Basic_AIScript>(EnemyClassLibrary.GetEnemyClassData(EnemyType.StudyBoss).GetLogicController());
	}
}
