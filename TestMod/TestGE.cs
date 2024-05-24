using RL2.ModLoader;
using RL2.ID;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace TestMod;

public class TestGE : GlobalEnemy
{
	public override Dictionary<int, EnemyRank[]> AppliesToEnemy => new Dictionary<int, EnemyRank[]>() {
		{ EnemyID.SpellswordBoss, new EnemyRank[] { EnemyRank.Basic } }
	};

	public override void OnSpawn() {
		Texture texture = TextureExtension.LoadTexture(TestMod.Location + "\\Assets\\Naameow.png");
		Texture texture2 = TextureExtension.LoadTexture(TestMod.Location + "\\Assets\\SpellSwordBoss_Texture.png");
		SwapTexture(texture2 as Texture2D, texture as Texture2D);
	}
}