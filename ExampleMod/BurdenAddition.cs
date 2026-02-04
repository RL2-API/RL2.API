using RL2.API;
using RL2.API.DataStructures;

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
			FlavourText = "222- Enemies take 200 damage less per level -222",
			IconPath = null,
			DefaultFoundState = FoundState.FoundAndViewed,
			Hint = "Monke",
		};

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