using RL2.ModLoader;

namespace TestMod;

public class TestPlayer : ModPlayer
{
	public static bool active;
	public override void ModifyStats()
	{
		if (active)
		{
			Player.statBonuses.maxMana += 40;
			Player.statBonuses.dashDistance += 16;
			Player.statBonuses.movementSpeed += 12;
		}
	}

	[Command("change-stats")]
	public static void AddMana()
	{
		active = true;
	}

	[Command("unchange-stats")]
	public static void SubMana()
	{
		active = false;
	}
}