using System.Collections.Generic;
using UnityEngine;

namespace RL2.ModLoader;

/// <summary>
/// Used to alter enemies. For creating new AIScript's use <see cref="BaseAIScript"/>, and change the enemy's script in here.
/// </summary>
public abstract class GlobalEnemy : ModType
{
	/// <summary>
	/// An EnemyController instance attached to this enemy.
	/// </summary>
	public EnemyController Enemy => gameObject.GetComponent<EnemyController>();

	/// <summary>
	/// The enemy type represented as an <see langword="int"/>.
	/// </summary>
	public int Type => (int)Enemy.EnemyType;
	
	/// <summary>
	/// The <see cref="EnemyRank"/> of the enemy.
	/// </summary>
	public EnemyRank Rank => Enemy.EnemyRank;
	
	/// <summary>
	/// Determines which enemies the instance of this GlobalEnemy will be attached to.<br></br>
	/// Leave empty to attach to every enemy.
	/// </summary>
	public virtual Dictionary<int, EnemyRank[]> AppliesToEnemy => new Dictionary<int, EnemyRank[]>();
	
	/// <summary>
	/// Ran on enemy spawn.
	/// </summary>
	public virtual void OnSpawn() { }
	
	/// <summary>
	/// Determines wether the affected enemy should die.
	/// </summary>
	/// <param name="killer">GameObject responsible for the enemy's death</param>
	/// <returns>Wether the enemy should die.</returns>
	public virtual bool PreKill(GameObject killer) => true;
	
	/// <summary>
	/// Ran immedieately after the enemy dies.
	/// </summary>
	public virtual void OnKill(GameObject killer) { }
}