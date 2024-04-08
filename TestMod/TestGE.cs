using RL2.ModLoader;

namespace TestMod;

public class TestGE : GlobalEnemy
{
	public override EnemyRank[] AppliesToEnemyRank => base.AppliesToEnemyRank;

	public void Update()
	{
		TestMod.Log("Spiuerldalal");
	}
}