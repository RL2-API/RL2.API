using RL2.API;
using RL2.API.DataStructures;
using UnityEngine;

public class RelicAddition : IRegistrable
{
	public static RelicType TestRelic;

	public void Register() {
		RelicData CustomData = ScriptableObject.CreateInstance<RelicData>();
		CustomData.Name = "Test Relic";
		CustomData.Rarity = 1;
		CustomData.MaxStack = 10;
		CustomData.CostType = RelicCostType.MaxHealthPermanent;
		CustomData.CostAmount = .15f;
		CustomData.Title = "Relic of Test";
		CustomData.Description = "Raa";
		CustomData.Description02 = "Ara ara";
		TestRelic = Relics.Register(CustomData);
		Player.PostUpdateStats += (PlayerController player) => {
			player.DexterityTemporaryAdd += SaveManager.PlayerSaveData.GetRelic(TestRelic).Level * 10;
		};

		Enemy.ModifyDamageTaken += (EnemyController enemyDamaged, IDamageObj damageSource, float damageTaken, ref Modifiers damageTakenModifiers, ref CriticalStrikeType critType, bool trueDamage) => {
			if (enemyDamaged.EnemyType != EnemyType.SwordKnight) return;
			damageTakenModifiers.Flat += 200 * SaveManager.PlayerSaveData.GetRelic(TestRelic).Level;
		};
	}
}