namespace RL2.API;

public class SetEnemiesDefeated_Rule : SetEnemiesDefeated_SummonRule {
	public EnemyType EnemyType { 
		get => m_enemyTypeDefeated;
		set => m_enemyTypeDefeated = value;
	}

	public EnemyRank EnemyRank { 
		get => m_enemyRankDefeated;
		set => m_enemyRankDefeated = value;
	}

	public int TimesDefeated { 
		get => m_timesDefeated;
		set => m_timesDefeated = value;
	}

	public bool Additive {
		get => m_additive;
		set => m_additive = value;
	}
}
