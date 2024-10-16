using MonoMod.RuntimeDetour;
using System;
using System.Reflection;

namespace RL2.ModLoader;

public partial class RL2API 
{
	internal static Hook OnGameLoad_Hook = new Hook(
		typeof(GameManager).GetMethod("Run", BindingFlags.NonPublic | BindingFlags.Instance),
		(Action orig) => {
			orig();
			foreach (Mod mod in LoadedMods) {
				mod.RegisterContent();
			}
		},
		new HookConfig() {
			ID = "RL2.API::RegisterContent",
			After = ["RL2.ModLoader::OnGameLoad"] 
		}
	);
}