using RL2.ModLoader;

namespace TestMod;

public class TestGE : GlobalEnemy
{
	public void OnEnable()
	{
		Mod.Log(Enemy.EnemyType.ToString());
	}
}
