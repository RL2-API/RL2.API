namespace RL2.API;

public class EndChallenge_Rule : EndChallenge_SummonRule {
	public ChallengeType ChallengeType {
		get => m_challengeType;
		set => m_challengeType = value;
	}
}
