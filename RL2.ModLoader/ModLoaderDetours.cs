using MonoMod.RuntimeDetour;
using System;
using System.Reflection;

namespace RL2.ModLoader;

public partial class ModLoader
{
	public static Hook VersionDisplay = new Hook(
		typeof(System_EV).GetMethod("GetVersionString", BindingFlags.Public | BindingFlags.Static),
		(Func<string> orig) => {
			return orig() + " | RL2.ModLoader v." + ModLoaderVersion.ToString();
		}
	);
}