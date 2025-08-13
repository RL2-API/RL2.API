namespace RL2.API.SummonRules;

/// <summary>
/// Awards the player a Heirloom once triggered
/// </summary>
public class AwardHeirloom : AwardHeirloom_SummonRule
{
	/// <summary>
	/// Which Heirloom to give to the player
	/// </summary>
	public HeirloomType HeirloomType { 
		get => m_heirloomType; 
		set => m_heirloomType = value; 
	}
}