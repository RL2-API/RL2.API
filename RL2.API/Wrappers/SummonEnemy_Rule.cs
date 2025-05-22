namespace RL2.API;

public class SummonEnemy_Rule : SummonEnemy_SummonRule
{
	public bool SpawnFast {
		get => m_spawnFast;
		set => m_spawnFast = value;
	}
	
	public bool SpawnAsCommander {
		get => m_summonAsCommander;
		set => m_summonAsCommander = value;
	}
	
	public bool RandomizeOnce {
		get => m_randomizeEnemiesOnce;
		set => m_randomizeEnemiesOnce = value;
	}

	public float SummonDelay { 
		get => m_summonDelay; 
		set => m_summonDelay = value; 
	}
	
	public float SummonValue { 
		get => m_summonValue; 
		set => m_summonValue = value; 
	}
}