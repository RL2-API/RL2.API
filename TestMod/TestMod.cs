using RL2.ModLoader;
using System;
using System.Security.Policy;
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

	[Command("set-money")]
	public static void MoneyCommand(string[] args)
	{
		if(args.Length == 0)
		{
			Log("You need to specify the amount of money you want");
			return;
		}
		if (!int.TryParse(args[0], out int money))
		{
			Log("The amount of money must be an integer");
			return;
		}
		SaveManager.PlayerSaveData.GoldCollected = money;
	}

	[Command("get-heirlooms")]
	public static void GetHeirlooms(string[] args)
	{
		foreach (HeirloomType type in typeof(HeirloomType).GetEnumValues())
		{
			if (type == HeirloomType.None) continue;
            SaveManager.PlayerSaveData.SetHeirloomLevel(type, 1, false, true);
        }
    }
}