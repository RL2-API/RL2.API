using RL2.ModLoader;

namespace TestMod;

public class TestDisabledModPlayer : ModPlayer
{
	public override void OnLoad() {
		Mod.Log("Shit don't work");
	}

	public override bool IsLoadingEnabled() => false;
}