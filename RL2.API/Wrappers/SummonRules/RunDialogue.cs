namespace RL2.API;

public class RunDialogue_Rule : RunDialogue_SummonRule {
	public string TitleTextLocID { 
		get => m_titleTextLocID; 
		set => m_titleTextLocID = value; 
	}

	public string BodyTextLocID {
		get => m_bodyTextLocID;
		set => m_bodyTextLocID = value;
	}
}
