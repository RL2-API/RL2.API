namespace RL2.API;

public static partial class SummonRule {
	/// <summary>
	/// Logs information to the log file
	/// </summary>
	public class DebugTrace : DebugTrace_SummonRule
	{
		/// <summary>
		/// The logged message
		/// </summary>
		public string Message { 
			get => m_debugString; 
			set => m_debugString = value; 
		}
	}
}