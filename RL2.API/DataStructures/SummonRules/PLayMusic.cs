namespace RL2.API.SummonRules;

/// <summary>
/// PLays music
/// </summary>
public class PLayMusic : PlayMusic_SummonRule {
	/// <summary>
	/// Which track to play
	/// </summary>
	public SongID SongID {
		get => m_songID; 
		set => m_songID = value;
	}
}
