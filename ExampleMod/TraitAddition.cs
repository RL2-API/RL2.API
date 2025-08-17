using RL2.API;
using System.Collections;
using UnityEngine;

namespace ExampleMod;

public class ExampleRegisterer : IRegistrable {
	public static TraitType MyTrait;

	void IRegistrable.Register() {
		TraitData data = ScriptableObject.CreateInstance<TraitData>();
		data.Name = "TestTrait";
		data.Title = "Testfull";
		data.Description = "Mmm";
		data.Description_2 = "Awooga";
		data.Rarity = 1;
		data.GoldBonus = .1f;

		Traits.LoadContent.Event += () => {
			MyTrait = Traits.Register<TraitAddition>(data);
		};

		Player.HeirGeneration.ModifyCharacterData.Event += ModifyCharacterData_Event;

		Player.PostUpdateStats.Event += PostUpdateStats_Event;
	}

	private void PostUpdateStats_Event(PlayerController player) {
		player.CritChanceTemporaryAdd += .1f;
	}

	private void ModifyCharacterData_Event(CharacterData data, bool classLocked, bool spellLocked) {
		Mod.Log(MyTrait);
		data.TraitOne = ExampleRegisterer.MyTrait;
	}
}

public class TraitAddition : BaseTrait {
	public override TraitType TraitType => ExampleRegisterer.MyTrait;

	IEnumerator Start() {
		WaitRL_Yield wait = new WaitRL_Yield(5);
		wait.Reset();
		yield return wait;
		Mod.Log("Mamałyga");
	}
}
