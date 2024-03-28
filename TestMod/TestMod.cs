using RL2.ModLoader;
using System;
using UnityEngine;

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
	public static void ShowFpsCommand(string[] args)
	{
		Messenger<DebugMessenger, DebugEvent>.Broadcast(DebugEvent.ToggleFPSCounter, null, null);
	}

	[Command("args-example")]
	public static void ArgsCommand(string[] args)
	{
		foreach (string arg in args)
		{
			Log(arg);
		}
	}
}