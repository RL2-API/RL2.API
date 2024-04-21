using RL2.ModLoader;

namespace TestMod;

public class TestMod : Mod
{
	public override int[] Version => new int[] { 0, 0, 1, 0 };
	public override void OnLoad() { 
		Log("CawaBunga");
	}
}