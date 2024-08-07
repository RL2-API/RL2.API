using RL2.ModLoader;

namespace ExampleMod;

public class ExampleModPlayer : ModPlayer {
	public override void ModifyStats() {
		StatBonuses.Strength += 20;
	}
}