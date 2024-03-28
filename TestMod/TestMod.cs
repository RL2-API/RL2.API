using RL2.ModLoader;
using System;

namespace TestMod;

public class TestMod : Mod
{
	public override void Load()
	{
		Messenger<ModMessenger, ModLoaderEvent>.AddListener(ModLoaderEvent.Load, OnLoad);
		Messenger<ModMessenger, ModLoaderEvent>.AddListener(ModLoaderEvent.Unload, OnUnload);
		Log($"TestMod loaded!");
	}

	public void OnLoad(MonoBehaviour sender, EventArgs eventArgs)
	{
		Log("Cawabunga");
	}

	public void OnUnload(MonoBehaviour sender, EventArgs eventArgs)
	{
		Log($"TestMod unloaded!");
	}

	[Command("fps")]
	public static void ShowFpsCommand()
	{
		Messenger<DebugMessenger, DebugEvent>.Broadcast(DebugEvent.ToggleFPSCounter, null, null);
	}
}