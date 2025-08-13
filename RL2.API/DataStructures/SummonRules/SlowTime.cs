namespace RL2.API.SummonRules;

/// <summary>
/// Triggers slowed time
/// </summary>
public class SlowTime : SlowTime_SummonRule {
	/// <summary>
	/// How fast timem will be moving
	/// </summary>
	public float SlowAmount {
		get => m_slowAmount;
		set => m_slowAmount = value;
	}

	/// <summary>
	/// How long the slowdown will last
	/// </summary>
	public float SlowDuration {
		get => m_slowDuration;
		set => m_slowDuration = value;
	}
}