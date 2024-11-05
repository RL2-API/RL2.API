using RL2.API;

namespace ExampleMod;

public class EnemyAlterations : IRegistrable
{
	public void Register() {
		// Every enemy can spawn in the Castle
		Enemy.ModifyData += (EnemyType type, EnemyRank rank, EnemyData data) => {
			data.SpawnInCastle = true;
		};
	}
}