using RL2.ModLoader;
using System.Linq;

namespace TestMod;

public class TestGE : GlobalEnemy
{
	public override EnemyType[] AppliesToEnemyType => new EnemyType[] { EnemyType.SpellswordBoss };
}
