namespace RL2.API;

public static partial class SummonRule {
	/// <summary>
	/// Mark enmies as defeated in the parade
	/// </summary>
	public class SetEnemiesDefeated : SetEnemiesDefeated_SummonRule {
		/// <summary>
		/// Type of the enemy
		/// </summary>
		public EnemyType EnemyType { 
			get => m_enemyTypeDefeated;
			set => m_enemyTypeDefeated = value;
		}

		/// <summary>
		/// Tier of the enemy
		/// </summary>
		public EnemyRank EnemyRank { 
			get => m_enemyRankDefeated;
			set => m_enemyRankDefeated = value;
		}

		/// <summary>
		/// Number of kills
		/// </summary>
		public int TimesDefeated { 
			get => m_timesDefeated;
			set => m_timesDefeated = value;
		}

		/// <summary>
		/// Whether to add to the current value in the parade, or set it to the provided value
		/// </summary>
		public bool Additive {
			get => m_additive;
			set => m_additive = value;
		}
	}
}
