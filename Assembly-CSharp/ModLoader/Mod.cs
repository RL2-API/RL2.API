using System;

namespace RL2.ModLoader;

public abstract class Mod
{
	public abstract int[] Version { get; }

	internal Type[] ModSystems;

	internal Type[] ModPlayers;

	public void OnLoad() { }

	public void OnUnload() { }
}
