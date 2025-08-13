namespace RL2.API;

public static partial class SummonRule {
	/// <summary>
	/// Kills all enemies in the room
	/// </summary>
	public class KillAllEnemies : KillAllEnemies_SummonRule {
		/// <summary>
		/// Whether to kill all non-summoned enemies
		/// </summary>
		public bool KillAllNonSummonedEnemies {
			get => m_killAllNonSummonedEnemies;
			set => m_killAllNonSummonedEnemies = value;
		}

		/// <summary>
		/// Whether to kill all summoned enemies
		/// </summary>
		public bool KillAllSummonedEnemies {
			get => m_killAllSummonedEnemies;
			set => m_killAllSummonedEnemies = value;
		}
	}
}
