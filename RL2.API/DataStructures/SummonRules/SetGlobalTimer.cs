namespace RL2.API;

public static partial class SummonRule {
	/// <summary>
	/// Can triggers several actions on the global timer, which broadcast their respective events on <see cref="UIMessenger"/>
	/// </summary>
	public class SetGlobalTimer : SetGlobalTimer_SummonRule {
		/// <summary>
		/// Whether the timer should be started <br></br>
		/// Broadcasts <see cref="UIEvent.StartGlobalTimer"/> with no <see cref="System.EventArgs"/> provided
		/// </summary>
		public bool StartTimer {
			get => m_startTimer;
			set => m_startTimer = value;
		}

		/// <summary>
		/// Whether the timer should be stopeed <br></br>
		/// Broadcasts <see cref="UIEvent.StopGlobalTimer"/> with no <see cref="System.EventArgs"/> provided
		/// </summary>
		public bool StopTimer {
			get => m_stopTimer;
			set => m_stopTimer = value;
		}

		/// <summary>
		/// Whether the timer should be displayed <br></br>
		/// Broadcasts <see cref="UIEvent.DisplayGlobalTimer"/> with no <see cref="System.EventArgs"/> provided
		/// </summary>
		public bool DisplayTimer {
			get => m_displayTimer;
			set => m_displayTimer = value;
		}

		/// <summary>
		/// Whether the timer should be hidden <br></br>
		/// Broadcasts <see cref="UIEvent.HideGlobalTimer"/> with no <see cref="System.EventArgs"/> provided
		/// </summary>
		public bool HideTimer {
			get => m_hideTimer;
			set => m_hideTimer = value;
		}

		/// <summary>
		/// Whether the timer should be reset <br></br>
		/// Broadcasts <see cref="UIEvent.ResetGlobalTimer"/> with no <see cref="System.EventArgs"/> provided
		/// </summary>
		public bool ResetTimer {
			get => m_resetTimer;
			set => m_resetTimer = value;
		}
	}
}
