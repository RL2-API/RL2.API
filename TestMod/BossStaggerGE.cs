using RL2.ModLoader;
using RL2.ModLoader.Sets;
using Steamworks;
using System;
using UnityEngine;

namespace TestMod;

public class BossStaggerGE : GlobalEnemy
{
	public override EnemyType[] AppliesToEnemyType => EnemySets.Bosses;

	const int StaggerRequirement = 5;
	public int CurrentHitCount = 0;
	public int StaggerHitCooldown = 0;

	public void Awake()
	{
		Messenger<GameMessenger, GameEvent>.AddListener(GameEvent.EnemyHit, OnHitBoss);
	}
	public void OnDestroy() { }

	public void OnHitBoss(MonoBehaviour sender, EventArgs eventArgs)
	{ 
		if ((eventArgs as CharacterHitEventArgs).Victim != Enemy)
		{
			return;
		}

		CurrentHitCount++;
		StaggerHitCooldown = 120;
	}
}