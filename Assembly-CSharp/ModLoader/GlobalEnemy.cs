using RL2.ModLoader.Sets;
using UnityEngine;

namespace RL2.ModLoader;

public abstract class GlobalEnemy : MonoBehaviour
{
	public EnemyController EnemyController => gameObject.GetComponent<EnemyController>();

	public BaseAIScript AIScript => gameObject.GetComponent<BaseAIScript>();

	public virtual EnemyType[] AppliesToEnemyType => EnemySets.AllEnemies;
	public virtual EnemyRank[] AppliesToEnemyRank => EnemySets.AllRanks;
}
