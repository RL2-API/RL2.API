namespace RL2.API;

public class SlowTime : SlowTime_SummonRule {

	public float SlowAmount {
		get => m_slowAmount;
		set => m_slowAmount = value;
	}

	public float SlowDuration {
		get => m_slowDuration;
		set => m_slowDuration = value;
	}
}
