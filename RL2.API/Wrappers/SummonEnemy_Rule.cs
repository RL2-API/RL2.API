// Wrapper calsses for X_SummonRule types

namespace RL2.API;

/// <summary>
/// <see cref="SummonRuleType"/>
/// </summary>
public class SummonEnemy_Rule : SummonEnemy_SummonRule
{
	/// <summary>
	/// 
	/// </summary>
	public bool SpawnFast {
		get => m_spawnFast;
		set => m_spawnFast = value;
	}
	
	/// <summary>
	/// 
	/// </summary>
	public bool SpawnAsCommander {
		get => m_summonAsCommander;
		set => m_summonAsCommander = value;
	}

	/// <summary>
	/// 
	/// </summary>
	public bool RandomizeOnce {
		get => m_randomizeEnemiesOnce;
		set => m_randomizeEnemiesOnce = value;
	}

	/// <summary>
	/// 
	/// </summary>
	public float SummonDelay { 
		get => m_summonDelay; 
		set => m_summonDelay = value; 
	}
	
	/// <summary>
	/// 
	/// </summary>
	public float SummonValue { 
		get => m_summonValue; 
		set => m_summonValue = value; 
	}
}