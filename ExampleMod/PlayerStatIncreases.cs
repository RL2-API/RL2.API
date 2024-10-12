using RL2.ModLoader;
using RL2.ModLoader.API;

namespace ExampleMod;

public class PlayerStatBoosts : IRegistrable
{ 
	void IRegistrable.Register() {
		// Increase strength scaling by 50%. This stacks additively with other Multiplier bonuses
		Player.Stats.StrengthMultiplier += (ref float multiplicative) => { 
			multiplicative += .5f; 
		};

		// Increase max health by 50
		Player.Stats.MaxHealthFlat += (ref int additive) => {
			additive += 50;
		};
	}
}