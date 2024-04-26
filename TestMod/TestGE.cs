using RL2.ID;
using RL2.ModLoader;
using System.Collections.Generic;

namespace TestMod;

public class TestGE : GlobalEnemy
{
	public override Dictionary<int, EnemyRank[]> AppliesToEnemy => new Dictionary<int, EnemyRank[]>() {
		{EnemyID.Starburst, new EnemyRank[] { EnemyRank.Basic, EnemyRank.Advanced }},
		{EnemyID.SwordKnight, new EnemyRank[] { EnemyRank.Basic, EnemyRank.Advanced, EnemyRank.Expert, EnemyRank.Miniboss } }
	};

	public override void OnSpawn()
	{
		Mod.Log("Works");
	}
}