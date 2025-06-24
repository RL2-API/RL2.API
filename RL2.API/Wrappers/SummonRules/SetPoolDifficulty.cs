namespace RL2.API;

/// <summary>Sets spawn pool difficulty</summary>
public class SetPoolDifficulty_Rule : SetSummonPoolDifficulty_SummonRule
{
	/// <summary>Enemy rank to spawn</summary>
	public EnemyRank Difficulty {
		get => m_summonPoolDifficulty;
		set => m_summonPoolDifficulty = value;
	}
}