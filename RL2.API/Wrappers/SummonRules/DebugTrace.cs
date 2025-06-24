namespace RL2.API;

public class DebugTrace_Rule : DebugTrace_SummonRule {
	public string Message { 
		get => m_debugString; 
		set => m_debugString = value; 
	}
}