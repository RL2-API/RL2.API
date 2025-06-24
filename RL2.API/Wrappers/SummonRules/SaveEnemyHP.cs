namespace RL2.API;

public class SaveEnemyHP_Rule : SaveCurrentEnemyHP_SummonRule {
	public bool IncludeRegularEnemies { 
		get => m_includeRegularEnemies; 
		set => m_includeRegularEnemies = value; 
	}
	public bool IncludeSummonedEnemies {
		get => m_includeSummonedEnemies;
		set => m_includeSummonedEnemies = value;
	}
}
