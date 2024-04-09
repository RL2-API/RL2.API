using RL2.ModLoader.Sets;
using System.Linq;
using UnityEngine;

namespace RL2.ModLoader;

public abstract class GlobalEnemy : MonoBehaviour
{
	private EnemyController[] ActiveEnemyControllers => PlayerManager.GetCurrentPlayerRoom().SpawnControllerManager.EnemySpawnControllers.Select(x => x.EnemyInstance).Where(x => AppliesToEnemyType.Contains(x.EnemyType) && AppliesToEnemyRank.Contains(x.EnemyRank)).ToArray();
	private BaseAIScript[] ActiveAIScripts => ActiveEnemyControllers.Select(x => x.LogicController.LogicScript).ToArray();
	public virtual EnemyType[] AppliesToEnemyType => EnemySets.AllEnemies;
	public virtual EnemyRank[] AppliesToEnemyRank => EnemySets.AllRanks;
	public virtual bool ActiveInRedPortals => true;
	public EnemyController Enemy => ActiveEnemyControllers.First(x => x.GameObject == gameObject);
	public BaseAIScript AIScript => ActiveAIScripts.First(x => x.EnemyController.GameObject == gameObject);
}
