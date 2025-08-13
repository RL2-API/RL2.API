namespace RL2.API;

public static partial class SummonRule {
	/// <summary>
	/// Sets the pool of enemies to be spawned
	/// </summary>
	public class SetEnemyPool : SetSummonPool_SummonRule
	{
		/// <summary>
		/// Which enemies should be available for spawning <br></br>
		/// Exclusive with <see cref="IsBiomeSpecific"/>
		/// </summary>
		public EnemyTypeAndRank[] EnemyPool {
			get => m_enemiesToSummonArray;
			set => m_enemiesToSummonArray = value;
		}


		/// <summary>
		/// Whether the enmies should be exclusive to the current biome <br></br>
		/// Exclusive with <see cref="EnemyPool"/>
		/// </summary>
		public bool IsBiomeSpecific {
			get => m_poolIsBiomeSpecific;
			set => m_poolIsBiomeSpecific = value;
		}

		/// <summary>
		/// whether flying enemies only hsould be spawned <br></br>
		/// Only in effect when <see cref="IsBiomeSpecific"/> is set to true
		/// </summary>
		public bool FlyingOnly {
			get => m_spawnFlyingOnly;
			set => m_spawnFlyingOnly = value;
		}
	}
}