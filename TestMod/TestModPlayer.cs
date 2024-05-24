using RL2.ModLoader;
using UnityEngine;

namespace TestMod;

public class TestModPlayer : ModPlayer
{
	public override void ModifyStats() {
		PlayerController.StatBonuses.armor += 20;
		PlayerController.StatBonuses.armorMultiplier += .25f;
		PlayerController.StatBonuses.strength += 50;
		PlayerController.CurrentArmor += 20;
	}

	public override void OnLoad(){
		Mod.Log("Loaded TestModPlayer");
	}

	public override bool PreKill(GameObject killer) => false;
}