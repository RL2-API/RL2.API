using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RL2.API;

public class SetSpawnPoints_Rule : SetSpawnPoints_SummonRule {
	public int[] SpawnPoints { 
		get => m_spawnPointArray; 
		set => m_spawnPointArray = value;
	}
}
