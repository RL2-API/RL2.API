using System.Collections.Generic;
using UnityEngine;

namespace RL2.ModLoader;

public class GlobalEnemy : ModType
{
	public virtual Dictionary<int, EnemyRank[]> AppliesToEnemy => [];
	
	public virtual void OnSpawn() { }

	public virtual bool PreKill(GameObject killer) => true;

	public virtual void OnKill(GameObject killer) { }
}