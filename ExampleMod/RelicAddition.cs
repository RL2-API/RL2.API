using MonoMod.RuntimeDetour;
using RL2.API;
using System;
using System.Reflection;
using UnityEngine;

public class RelicAddition : IRegistrable {
	public static int TestRelic;

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
			player.DexterityTemporaryAdd += SaveManager.PlayerSaveData.GetRelic((RelicType)TestRelic).Level * 10;
		};
	}

	public Hook Test = new Hook(
		typeof(RelicRoomPropController).GetProperty("LeftRelicType", BindingFlags.Public | BindingFlags.Instance).GetMethod,
		(Func<RelicRoomPropController, RelicType> orig, RelicRoomPropController room) => {
			return TestRelic;
		}
	);
}