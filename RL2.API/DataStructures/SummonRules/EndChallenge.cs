namespace RL2.API.SummonRules;

/// <summary>
/// Ends the Scar Challenge
/// </summary>
public class EndChallenge : EndChallenge_SummonRule {
	/// <summary>
	/// Which Scar Challenge to mark as completed
	/// </summary>
	public ChallengeType ChallengeType {
		get => m_challengeType;
		set => m_challengeType = value;
	}
}
