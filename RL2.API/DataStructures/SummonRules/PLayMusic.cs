namespace RL2.API;

public static partial class SummonRule
{
	/// <summary>
	/// PLays music
	/// </summary>
	public class PLayMusic : PlayMusic_SummonRule
	{
		/// <summary>
		/// Which track to play
		/// </summary>
		public SongID SongID {
			get => m_songID;
			set => m_songID = value;
		}
	}
}