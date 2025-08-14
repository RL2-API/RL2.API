namespace RL2.API;

public static partial class SummonRule
{
	/// <summary>
	/// Triggers dialogue
	/// </summary>
	public class RunDialogue : RunDialogue_SummonRule
	{
		/// <summary>
		/// Title text localization ID
		/// </summary>
		public string TitleTextLocID {
			get => m_titleTextLocID;
			set => m_titleTextLocID = value;
		}

		/// <summary>
		/// Body text localization ID
		/// </summary>
		public string BodyTextLocID {
			get => m_bodyTextLocID;
			set => m_bodyTextLocID = value;
		}
	}
}