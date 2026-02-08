using RL2.API;
using RL2.API.DataStructures;
using UnityEngine;

public class BurdenAddition : IRegistrable
{
	public void Register() {

		Burdens.Data CustomData = new Burdens.Data() {
			Name = "Burden of No-U",
			MaxBurdenLevel = 10,
			InitialBurdenCost = 1,
			ScalingBurdenCost = 2,
			Disabled = false,
			StatsGain = .1f,
			Title = "Burden of No-U",
			Description = "Enemies take 200 damage less per level",
			FlavourText = "The Knight's armor protects all enemies",
			IconPath = null,
			DefaultFoundState = FoundState.NotFound,
			Hint = "A Knight guards his secrets well...",
		};

		Burdens.LoadContent.Event += () => {
			Burdens.Register(CustomData);
		};

		Enemy.OnKill.Event += (EnemyController enemy, GameObject killer) => {
			if (enemy.EnemyType != EnemyType.SwordKnight) return;

			BurdenType type = Burdens.GetType("ExampleMod/Burden of No-U");
			if (BurdenManager.GetFoundState(type) == FoundState.NotFound) {
				Mod.Log("Unlocking Burden");
				BurdenManager.SetFoundState(type, FoundState.FoundButNotViewed, overrideValues: true);
			}
		};

		Enemy.ModifyDamageTaken.Event += (EnemyController enemyDamaged, IDamageObj damageSource, float damageTaken, ref CriticalStrikeType critType, ref Modifiers damageTakenModifiers) => {
			int level = SaveManager.PlayerSaveData.GetBurden(Burdens.GetType("ExampleMod/Burden of No-U")).CurrentLevel;
			if (level > 0) {
				damageTakenModifiers.Flat -= 200 * level;				
			}
		};
	}
}