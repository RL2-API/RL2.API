namespace RL2.API;

public static partial class SummonRule {
	/// <summary>
	/// Set the level of enemies in the pool. <br></br>
	/// The displayed value will be multiplied by 2.5
	/// </summary>
	public class SetEnemyLevel : SetSummonPoolLevelMod_SummonRule {
		/// <summary>
		/// The enemies will have the same level as the room <br></br>
		/// Exclusive with <see cref="Level"/>
		/// </summary>
		public bool SetToRoomLevel {
			get => m_setLevelToRoom;
			set => m_setLevelToRoom = value;
		}

		/// <summary>
		/// The level of the enemies <br></br>
		/// Exclusive with <see cref="SetToRoomLevel"/>
		/// </summary>
		public int Level {
			get => m_levelMod;
			set => m_levelMod = value;
		}
	}
}