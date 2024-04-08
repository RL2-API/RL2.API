namespace RL2.ModLoader.Sets;

public class EnemySets
{
	public static readonly EnemyType[] AllEnemies = new EnemyType[] { EnemyType.Any };

	public static readonly EnemyType[] Bosses = new EnemyType[]
	{
		EnemyType.SpellswordBoss,
		EnemyType.SkeletonBossA, EnemyType.SkeletonBossB,
		EnemyType.DancingBoss,
		EnemyType.StudyBoss, EnemyType.MimicChestBoss,
		EnemyType.EyeballBoss_Left, EnemyType.EyeballBoss_Middle, EnemyType.EyeballBoss_Right, EnemyType.EyeballBoss_Bottom,
		EnemyType.CaveBoss,
		EnemyType.TraitorBoss,
		EnemyType.FinalBoss
	};

	public static readonly EnemyType[] Flying = new EnemyType[]
	{
		EnemyType.DancingBoss,
		EnemyType.ElementalBounce,
		EnemyType.ElementalCurse,
		EnemyType.ElementalDash,
		EnemyType.ElementalFire,
		EnemyType.ElementalIce,
		EnemyType.Eyeball,
		EnemyType.FlyingAxe,
		EnemyType.FlyingBurst,
		EnemyType.FlyingHammer,
		EnemyType.FlyingHunter,
		EnemyType.FlyingShield,
		EnemyType.FlyingSkull,
		EnemyType.FlyingSword,
		EnemyType.Ghost,
		EnemyType.PaintingEnemy,
		EnemyType.Sniper,
		EnemyType.Starburst,
		EnemyType.StudyBoss,
		EnemyType.Wisp
	};

	public static readonly EnemyRank[] AllRanks = new EnemyRank[] { EnemyRank.Any };
}
