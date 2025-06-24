namespace RL2.API;

public  class KillAllEnemies_Rule : KillAllEnemies_SummonRule {
	public bool KillAllNonSummonedEnemies {
		get => m_killAllNonSummonedEnemies;
		set => m_killAllNonSummonedEnemies = value;
	}

	public bool KillAllSummonedEnemies {
		get => m_killAllSummonedEnemies;
		set => m_killAllSummonedEnemies = value;
	}
}
