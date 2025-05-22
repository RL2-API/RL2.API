namespace RL2.API;

public class SetSummonPoolLevel_Rule : SetSummonPoolLevelMod_SummonRule 
{
	public bool SetLevelToPlayer {
		get => m_setLevelToRoom;
		set => m_setLevelToRoom = value;
	}

	public int Level {
		get => m_levelMod;
		set => m_levelMod = value;
	}
}
