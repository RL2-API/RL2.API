using System;

namespace RL2.ModLoader;

public abstract class Mod
{
	public abstract int[] Version { get; }

	internal static Type[] ModSystems;

	internal static Type[] ModPlayers;

	public void OnLoad() { }

	public void OnUnload() { }
}
