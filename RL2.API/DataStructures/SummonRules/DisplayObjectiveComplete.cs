namespace RL2.API;

public static partial class SummonRule
{
	/// <summary>
	/// Displays a banner across the screen
	/// </summary>
	public class DisplayObjectiveComplete : DisplayObjectiveComplete_SummonRule
	{
		/// <summary>
		/// What type of banner to use. <br></br> 
		/// <see cref="ObjectiveCompleteHUDType.Insight"/> and <see cref="ObjectiveCompleteHUDType.Boss"/> will trigger the <see cref="UIEvent.DisplayObjectiveCompleteHUD"/> event on <see cref="UIMessenger"/>
		/// </summary>
		public ObjectiveCompleteHUDType HUDType {
			get => m_hudType;
			set => m_hudType = value;
		}

		/// <summary>
		/// Which Insight was unlocked
		/// </summary>
		public InsightType InsightType {
			get => m_insightType;
			set => m_insightType = value;
		}

		/// <summary>
		/// Wether the Inisght was just dicovered
		/// </summary>
		public bool InsightDiscovered {
			get => m_insightDiscovered;
			set => m_insightDiscovered = value;
		}

		/// <summary>
		/// Boss message key in localization file
		/// </summary>
		public string BossLocIDOverride {
			get => m_bossLocIDOverride;
			set => m_bossLocIDOverride = value;
		}
	}
}