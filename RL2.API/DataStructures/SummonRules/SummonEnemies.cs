namespace RL2.API.SummonRules;

/// <summary>
/// Summons enemies
/// </summary>
public class SummonEnemies : SummonEnemy_SummonRule
{
	/// <summary>
	/// Whether to use the fast spawning
	/// </summary>
	public bool SpawnFast {
		get => m_spawnFast;
		set => m_spawnFast = value;
	}

	/// <summary>
	/// Whether the spawned enemy should be a commander
	/// </summary>
	public bool SpawnAsCommander {
		get => m_summonAsCommander;
		set => m_summonAsCommander = value;
	}

	/// <summary>
	/// Whether only one enemy from the pool will be chosen, or on each spawn attempt the chosen enemy will be rerolled
	/// </summary>
	public bool RandomizeOnce {
		get => m_randomizeEnemiesOnce;
		set => m_randomizeEnemiesOnce = value;
	}

	/// <summary>
	/// Delay between summoning next enemies
	/// </summary>
	public float SummonDelay {
		get => m_summonDelay;
		set => m_summonDelay = value;
	}

	/// <summary>
	/// Total summon value availble to spend. <br></br>
	/// Look at the specific enemy's <see cref="EnemyData.SummonValue"/> to figure out what to set it as. <br></br>
	/// Summoning will continue until there is no more <see cref="SummonValue"/> available or all enemies are too expensive to be spawned
	/// </summary>
	public float SummonValue {
		get => m_summonValue;
		set => m_summonValue = value;
	}
}