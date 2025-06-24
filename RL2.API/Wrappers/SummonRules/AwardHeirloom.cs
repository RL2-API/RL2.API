namespace RL2.API;

public class AwardHeirloom_Rule : AwardHeirloom_SummonRule
{
	public HeirloomType HeirloomType { 
		get => m_heirloomType; 
		set => m_heirloomType = value; 
	}
}