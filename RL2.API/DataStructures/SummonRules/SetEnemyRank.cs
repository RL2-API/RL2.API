namespace RL2.API.SummonRules;

/// <summary>
/// Sets the tier of summoned enemies
/// </summary>
public class SetEnemyRank : SetSummonPoolDifficulty_SummonRule {
	/// <summary>
	/// The rank to be forced onto the summoned enemy
	/// </summary>
	public EnemyRank Rank { 
		get => m_summonPoolDifficulty;
		set => m_summonPoolDifficulty = value;
	}
}
