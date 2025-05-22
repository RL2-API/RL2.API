namespace RL2.API;

public class SetPoolDifficulty_Rule : SetSummonPoolDifficulty_SummonRule 
{
	public EnemyRank Difficulty {
		get => m_summonPoolDifficulty;
		set => m_summonPoolDifficulty = value;
	}
}
