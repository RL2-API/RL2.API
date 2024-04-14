using RL2.ModLoader.Sets;
using System.Linq;
using UnityEngine;

namespace RL2.ModLoader;

public abstract class GlobalEnemy : MonoBehaviour
{
	/// <summary>
	/// Instance of the room the player and the enemy are currently in.
	/// </summary>
	public BaseRoom Room => PlayerManager.GetCurrentPlayerRoom();

	/// <summary>
	/// All instances of <see cref="EnemyController"/> in the Arena room the player is in.
	/// </summary>
	public EnemyController[] ArenaEnemyControllers => (from x in EnemyManager.SummonedEnemyList where x != null select x).ToArray();

	/// <summary>
	/// All instances of <see cref="EnemyController"/> in the standard room the player is in.
	/// </summary>
	public EnemyController[] RoomEnemyControllers => (from enemySpawnController 
														in Room.SpawnControllerManager.EnemySpawnControllers
														where enemySpawnController.EnemyInstance != null
														select enemySpawnController.EnemyInstance).ToArray();

	/// <summary>
	/// All instances of <see cref="EnemyController"/> connected with enemies in the current room that fullfil the criteria of <see cref="AppliesToEnemyType"/>, <see cref="AppliesToEnemyRank"/> and <see cref="AppliesToDeadEnemies"/>
	/// </summary>
	public EnemyController[] ActiveEnemeyControllers => (from enemy 
														in Room.SpecialRoomType == SpecialRoomType.Arena ? ArenaEnemyControllers : RoomEnemyControllers
														where AppliesToEnemyType.Contains(enemy.EnemyType) && AppliesToEnemyRank.Contains(enemy.EnemyRank) && (enemy.IsDead == AppliesToDeadEnemies | !enemy.IsDead)
														select enemy).ToArray();

	/// <summary>
	/// All instances of <see cref="BaseAIScript"/> connected with enemies in the current room that fullfil the criteria of <see cref="AppliesToEnemyType"/>, <see cref="AppliesToEnemyRank"/> and <see cref="AppliesToDeadEnemies"/>
	/// </summary>
	public BaseAIScript[] ActiveAIScripts => (from enemy in ActiveEnemeyControllers where enemy.LogicController.LogicScript != null select enemy.LogicController.LogicScript).ToArray();
	
	/// <summary>
	/// An instance of <see cref="EnemyController"/> connected with this enemy.
	/// </summary>
	public EnemyController Enemy => gameObject.GetComponent<EnemyController>();


	/// <summary>
	/// An instance of <see cref="BaseAIScript"/> connected with this enemy.
	/// </summary>
	public BaseAIScript AIScript => gameObject.GetComponent<BaseAIScript>();

	/// <summary>
	/// Specifies <see cref="EnemyType"/>s which will get an instance of this class attached to them. <br></br>
	/// Applies to all enemies by default.
	/// </summary>
	public virtual EnemyType[] AppliesToEnemyType => EnemySets.AllEnemies;

	/// <summary>
	/// Specifies <see cref="EnemyRank"/>s which will get an instance of this class attached to them. <br></br>
	/// Applies to all ranks by default.
	/// </summary>
	public virtual EnemyRank[] AppliesToEnemyRank => EnemySets.AllRanks;
	
	/// <summary>
	/// Specifies wether this script should run on enemies in Challenges. <br></br>
	/// Defaults to <see langword="true"/>
	/// </summary>
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
