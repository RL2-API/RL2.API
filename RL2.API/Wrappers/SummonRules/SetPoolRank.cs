namespace RL2.API;

public class SetPoolRank_Rule : SetSummonPoolDifficulty_SummonRule {
	public EnemyRank Rank { 
		get => m_summonPoolDifficulty;
		set => m_summonPoolDifficulty = value;
	}
}
