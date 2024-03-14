using RL2.ModLoader;

namespace TestMod;

public class TestMod : Mod
{
    public override void OnLoad()
    {
        ModLoader.Log("TestMod loaded and OnLoad was ran!");
    }
}