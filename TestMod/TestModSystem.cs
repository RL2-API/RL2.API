using RL2.ModLoader;

namespace TestMod;

public class TestModSystem : ModSystem
{
	public override void OnLoad() {
		Mod.Log("testmodsystemLoaded");
	}
}