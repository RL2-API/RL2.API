using RL2.ModLoader;
using UnityEngine;

namespace ExampleMod;

public class ExampleGlobalEnemy : GlobalEnemy {
	public override bool AppliesToEnemy(int enemyType, EnemyRank rank) {
		return enemyType == (int)EnemyType.SpellswordBoss;
	}

	public override void OnSpawn() {
		Texture2D oldTexture = TextureExtension.LoadTexture(RL2API.GetModInstance<ExampleMod>().Path + "Assets\\SpellSwordBoss_Texture.png");
		Texture2D newTexture = TextureExtension.LoadTexture(RL2API.GetModInstance<ExampleMod>().Path + "Assets\\Naameow.png");
		SwapTexture(oldTexture, newTexture, true);
	}
}