﻿using RL2.ModLoader;
using RL2.ModLoader.Sets;
using System;
using UnityEngine;

namespace TestMod;

public class BossStaggerGE : GlobalEnemy
{
	public override EnemyType[] AppliesToEnemyType => EnemySets.Bosses;

	const int StaggerRequirement = 5;
	public int CurrentHitCount = 0;
	public int StaggerHitCooldown = 0;
	public int StaggerWindow = 0;

	public void Awake()
	{
		Messenger<GameMessenger, GameEvent>.AddListener(GameEvent.EnemyHit, OnHitBoss);
    }
	public void OnDisable()
	{
		Messenger<GameMessenger, GameEvent>.RemoveListener(GameEvent.EnemyHit, OnHitBoss);
	}

	public void OnHitBoss(MonoBehaviour sender, EventArgs eventArgs)
	{
		if (!((eventArgs as CharacterHitEventArgs).Victim as EnemyController).IsBoss)
		{
			return;
		}
		if ((eventArgs as CharacterHitEventArgs).Victim != Enemy)
		{
			return;
		}
		if (StaggerWindow != 0)
		{
			Enemy.SetHealth((eventArgs as CharacterHitEventArgs).DamageTaken * -8, true, true);
			Enemy.TargetController.SetHealth((eventArgs as CharacterHitEventArgs).DamageTaken / 10, true, true);
			StaggerWindow = 0;
            EffectManager.PlayEffect(gameObject, Enemy.Animator, "ScreenFlashEffect", Vector3.zero, 0f, EffectStopType.Immediate);
            return;
		}
		CurrentHitCount++;
		StaggerHitCooldown = 120;
	}

	public void Update()
	{
		ModLoader.Log(Enemy.name);
		if (StaggerHitCooldown > 0)
		{
			StaggerHitCooldown--;
		}
		if (StaggerHitCooldown == 0)
		{
			CurrentHitCount = 0;
		}

		if (CurrentHitCount == StaggerRequirement)
		{
			StaggerWindow = 120;
		}

		if (StaggerWindow != 0)
        {
			if (!AIScript.IsPaused)
            {
				AIScript.Pause();
                EffectManager.PlayEffect(gameObject, Enemy.Animator, "ScreenFlashEffect", Vector3.zero, 0f, EffectStopType.Gracefully);
            }
            CurrentHitCount = 0;
            StaggerWindow--;
			return;
		}

		if (AIScript.IsPaused && !GameManager.IsGamePaused)
		{
			AIScript.Unpause();
		}
	}
}