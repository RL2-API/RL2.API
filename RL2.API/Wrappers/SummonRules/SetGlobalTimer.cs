namespace RL2.API;

public class SetGlobalTimer_Rule : SetGlobalTimer_SummonRule {
	public bool StartTimer {
		get => m_startTimer;
		set => m_startTimer = value;
	}

	public bool StopTimer {
		get => m_stopTimer;
		set => m_stopTimer = value;
	}

	public bool DisplayTimer {
		get => m_displayTimer;
		set => m_displayTimer = value;
	}

	public bool HideTimer {
		get => m_hideTimer;
		set => m_hideTimer = value;
	}

	public bool ResetTimer {
		get => m_resetTimer;
		set => m_resetTimer = value;
	}

	public float SlowDuration {
		get => m_slowDuration;
		set => m_slowDuration = value;
	}
}
