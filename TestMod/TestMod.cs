using RL2.ModLoader;

namespace TestMod;

public class TestMod : Mod
{
	public override void OnLoad() { 
		Log("CawaBunga");
	}

	[Command("set-gold")]
	public static void SetGold(string[] args)
	{
		if (args.Length == 0)
		{
			Log("You need to specify the amount of gold you want");
			return;
		}
		if (!int.TryParse(args[0], out int money))
		{
			Log("The amount of gold must be an integer");
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