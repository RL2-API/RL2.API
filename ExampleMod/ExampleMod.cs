using RL2.ModLoader;

namespace ExampleMod;

public class ExampleMod : Mod 
{
	public override void OnLoad() {
		Mod.Log("ExampleMod loaded");
	}

	[Command("Test")]
	public static void TestCommand(string[] args) {
		Mod.Log("Waka waka");
	}
}