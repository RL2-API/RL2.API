using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RL2.API;

public class SetPoolLevel_Rule : SetSummonPoolLevelMod_SummonRule {
	public bool SetToRoomLevel {
		get => m_setLevelToRoom;
		set => m_setLevelToRoom = value;
	}

	public int Level {
		get => m_levelMod;
		set => m_levelMod = value;
	}
}
