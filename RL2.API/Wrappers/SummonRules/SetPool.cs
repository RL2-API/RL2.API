namespace RL2.API;

public class SetPool_Rule : SetSummonPool_SummonRule
{
	public EnemyTypeAndRank[] EnemyPool {
		get => m_enemiesToSummonArray;
		set => m_enemiesToSummonArray = value;
	}

	public bool IsBiomeSpecific {
		get => m_poolIsBiomeSpecific;
		set => m_poolIsBiomeSpecific = value;
	}

	public bool FlyingOnly {
		get => m_spawnFlyingOnly;
		set => m_spawnFlyingOnly = value;
	}
}