using RL2.ModLoader;
using System;
using UnityEngine;

namespace TestMod;

public class TestGE : GlobalEnemy
{
	public override EnemyType[] AppliesToEnemyType => new EnemyType[] { EnemyType.SpellswordBoss };

	Texture2D texture;

	public override void OnSpawn()
	{
		texture = new Texture2D(1, 1);
		texture.SetPixel(1, 1, Color.blue);
		try
		{

            Enemy.SwapTexture(0, texture);
        }
		catch (Exception ex)
		{
			Mod.Log($"Failed to swap texture: {ex}");
		}
	}
}
