using RL2.ModLoader.Sets;
using System.Linq;
using UnityEngine;

namespace RL2.ModLoader;

public abstract class GlobalEnemy : MonoBehaviour
{
	public BaseRoom Room => PlayerManager.GetCurrentPlayerRoom();
	public EnemyController[] ArenaEnemyControllers => (from x in EnemyManager.SummonedEnemyList where x != null select x).ToArray();
	
	public EnemyController[] RoomEnemyControllers => (from enemySpawnController 
														in Room.SpawnControllerManager.EnemySpawnControllers
														where enemySpawnController.EnemyInstance != null
														select enemySpawnController.EnemyInstance).ToArray();
	
	public EnemyController[] ActiveEnemeyControllers => (from enemy 
														in Room.SpecialRoomType == SpecialRoomType.Arena ? ArenaEnemyControllers : RoomEnemyControllers
														where AppliesToEnemyType.Contains(enemy.EnemyType) && AppliesToEnemyRank.Contains(enemy.EnemyRank) && !enemy.IsDead 
														select enemy).ToArray();

	public BaseAIScript[] ActiveAIScripts => (from enemy in ActiveEnemeyControllers where enemy.LogicController.LogicScript != null select enemy.LogicController.LogicScript).ToArray();
	public EnemyController Enemy => ActiveEnemeyControllers.First(x => x.GameObject == gameObject);
	public BaseAIScript AIScript => ActiveAIScripts.First(x => x.EnemyController.GameObject == gameObject);
	public virtual EnemyType[] AppliesToEnemyType => EnemySets.AllEnemies;
	public virtual EnemyRank[] AppliesToEnemyRank => EnemySets.AllRanks;
	public virtual bool ActiveInRedPortals => true;

	/// <summary>
	/// Specifies wether this script should run on dead enemies. <br></br>
	/// Defaults to <see langword="false"/>
	/// </summary>
	public virtual bool AppliesToDeadEnemies => false;
	
	/// <summary>
	/// Ran right after spawning the enemy
	/// </summary>
	public virtual void OnSpawn() { }
}
