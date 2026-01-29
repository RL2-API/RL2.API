using RL2.API;
using RL2.API.DataStructures;
using UnityEngine;
using static Mono.Security.X509.X520;

public class BurdenAddition : IRegistrable
{
	public void Register() {

		BurdenData CustomData = ScriptableObject.CreateInstance<BurdenData>();
		CustomData.Name = "Burden of No-U";
		CustomData.MaxBurdenLevel = 10;
		CustomData.InitialBurdenCost = 1;
		CustomData.ScalingBurdenCost = 2;
		CustomData.Disabled = false;
		CustomData.StatsGain = .1f;
		CustomData.Title = "Burden of No-U";
		CustomData.Description = "Enemies take 200 damage less per level";
		CustomData.Description2 = "222- Enemies take 200 damage less per level -222";
		CustomData.Hint = "Monke";

		Burdens.LoadContent.Event += () => {
			Burdens.Register(CustomData);
		};

		Enemy.ModifyDamageTaken.Event += (EnemyController enemyDamaged, IDamageObj damageSource, float damageTaken, ref CriticalStrikeType critType, ref Modifiers damageTakenModifiers) => {
			int level = SaveManager.PlayerSaveData.GetBurden(Burdens.GetType("ExampleMod/Burden of No-U")).CurrentLevel;
			if (level > 0) { }
			damageTakenModifiers.Flat -= 200 * level;
		};
	}
}