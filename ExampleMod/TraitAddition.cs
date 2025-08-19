using RL2.API;
using System.Collections;
using UnityEngine;

namespace ExampleMod;

public class ExampleTrait : BaseTrait, IRegistrable
{
	public static TraitType MyTrait;
	public override TraitType TraitType => MyTrait;

	void IRegistrable.Register() {
		TraitData data = ScriptableObject.CreateInstance<TraitData>();
		data.Name = "TestTrait";
		data.Title = "Testfull";
		data.Description = "Mmm";
		data.Description_2 = "Awooga";
		data.Rarity = 1;
		data.GoldBonus = .1f;

		Traits.LoadContent.Event += () => {
			MyTrait = Traits.Register<ExampleTrait>(data);
		};

		Player.HeirGeneration.ModifyCharacterData.Event += ModifyCharacterData_Event;

		Player.PostUpdateStats.Event += PostUpdateStats_Event;

		Traits.ApplyEffect.Event += ApplyEffect_Event;
	}

	void Awake() {
		if (!PlayerManager.IsInstantiated) return;

		Mod.Log("Test Trait instantiated!");
	}

	private void ApplyEffect_Event(TraitType type) {
		if (type != MyTrait) return;

		Mod.Log("Applied TestTrait");
	}

	private void PostUpdateStats_Event(PlayerController player) {
		if (TraitManager.IsTraitActive(MyTrait))
			player.CritChanceTemporaryAdd += .1f;
	}

	private void ModifyCharacterData_Event(CharacterData data, bool classLocked, bool spellLocked) {
		if (classLocked)
			data.TraitOne = MyTrait;
	}
}