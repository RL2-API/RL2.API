using RL2.ModLoader;

namespace TestMod;

public class TestModPlayer : ModPlayer
{
	public override void OnRelicChanged(RelicType relicType) {
		Mod.Log(relicType.ToString());
	}

	public override void OnLoad(){
		Mod.Log("Loaded TestModPlayer");
	}
}