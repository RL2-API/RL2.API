using RL2.ModLoader;

namespace TestMod;

public class TestGE : GlobalEnemy
{
	public override EnemyType[] AppliesToEnemyType => new EnemyType[] { EnemyType.SpellswordBoss };

	int Timer = 0;

	public void Update()
	{
		Timer++;
		if (Timer >= 600)
		{
			Enemy.SetLevel(10000);
		}
	}
}
