namespace RL2.API;

/// <summary>
/// Sets enemy level. Enemy level is calculated via <code>room_level * 2.5</code>
/// </summary>
public class SetEnemyLevel_Rule : SetSummonPoolLevelMod_SummonRule 
{
	/// <summary>Overrides room level to be the same as the player</summary>
	public bool SetLevelToPlayer {
		get => m_setLevelToRoom;
		set => m_setLevelToRoom = value;
	}

	/// <summary>Sets the room level</summary>
	public int Level {
		get => m_levelMod;
		set => m_levelMod = value;
	}
}
