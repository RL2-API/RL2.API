namespace RL2.API.SummonRules;

/// <summary>
/// Sets available spawn points
/// </summary>
public class SetSpawnPoints : SetSpawnPoints_SummonRule {
	/// <summary>
	/// Which spawn points will be available
	/// </summary>
	public int[] SpawnPoints { 
		get => m_spawnPointArray; 
		set => m_spawnPointArray = value;
	}
}
