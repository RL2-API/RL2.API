using API = RL2.API;

public struct SoulShopAdd : API.IRegistrable {
	void API.IRegistrable.Register() {
		API.SoulShop.LoadContent.Event += () => {
			API.SoulShop.Register(
				new API.SoulShop.Data() {
					Description = "AA",
					AdditionalLevelStatGain = 1,
					BaseCost = 0,
					Disabled = false,
					FirstLevelStatGain = 10,
					UnlockLevel = 10,
					MaxLevel = 10,
					MaxSoulCostCap = 400,
					Name = "aaaa",
					OverloadMaxLevel = 20,
					ScalingCost = 50,
					StatsTitle = "WAHOO",
					Title = "Nyeh Nyeh Nyeh",
				}, 
				new API.SoulShop.Entry() { 
					IsToggle = false,				}
			);
			
			API.SoulShop.Register(
				new API.SoulShop.Data() {
					Description = "The second Heir is blessed with Knoweledge from the Ninja",
					AdditionalLevelStatGain = 1,
					BaseCost = 0,
					Disabled = false,
					FirstLevelStatGain = 1,
					UnlockLevel = 10,
					MaxLevel = 1,
					Name = "AaaA",
					OverloadMaxLevel = 1,
					StatsTitle = "WAHOO unlocker!",
					Title = "Toggleable Nyeh Nyeh Nyeh",
				}, 
				new API.SoulShop.Entry() { 
					IsToggle = true,
				}
			);
		};

		API.Player.HeirGeneration.ModifyCharacterData.Event_v2 += (CharacterData data, int index) => {
			if (SaveManager.ModeSaveData.GetSoulShopObj(API.SoulShop.GetType("ExampleMod/AaaA")).CurrentEquippedLevel > 0) {
				if (index != 1) return; 
				data.Weapon = AbilityType.KunaiWeapon;
			}
		};
	}
}