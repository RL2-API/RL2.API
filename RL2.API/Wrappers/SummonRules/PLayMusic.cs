namespace RL2.API;

public class PLayMusic_Rule : PlayMusic_SummonRule {
	public SongID SongID {
		get => m_songID; 
		set => m_songID = value;
	}
}
