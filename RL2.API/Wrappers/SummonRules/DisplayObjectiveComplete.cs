namespace RL2.API;

public class DisplayObjectiveComplete_Rule : DisplayObjectiveComplete_SummonRule {
	public ObjectiveCompleteHUDType HUDType { 
		get => m_hudType; 
		set => m_hudType = value;
	}

	public InsightType InsightType { 
		get => m_insightType; 
		set => m_insightType = value;
	}

	public bool InsightDiscovered { 
		get => m_insightDiscovered; 
		set => m_insightDiscovered = value; 
	}

	public string BossLocIDOverride { 
		get => m_bossLocIDOverride; 
		set => m_bossLocIDOverride = value;
	}

	public InsightObjectiveCompleteHUDEventArgs InsightCompleteEventArgs {
		get => m_insightCompleteEventArgs;
		set => m_insightCompleteEventArgs = value;
	}

	public BossObjectiveCompleteHUDEventArgs BossCompleteEventArgs {
		get => m_bossCompleteEventArgs;
		set => m_bossCompleteEventArgs = value;
	}
}
