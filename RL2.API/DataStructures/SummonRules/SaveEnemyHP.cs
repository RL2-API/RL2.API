namespace RL2.API;

public static partial class SummonRule
{
	/// <summary>
	/// Save the total HP of present enemies into <see cref="SummonRuleController.SavedCurrentEnemyHP"/>
	/// </summary>
	public class SaveEnemyHP : SaveCurrentEnemyHP_SummonRule
	{
		/// <summary>
		/// Whether to count the HP of normal enemies
		/// </summary>
		public bool IncludeRegularEnemies {
			get => m_includeRegularEnemies;
			set => m_includeRegularEnemies = value;
		}
		/// <summary>
		/// Whether to count the HP of summoned enemies
		/// </summary>
		public bool IncludeSummonedEnemies {
			get => m_includeSummonedEnemies;
			set => m_includeSummonedEnemies = value;
		}
	}
}