using MonoMod.RuntimeDetour;
using System;
using System.Reflection;

namespace RL2.API;

public partial class RL2API 
{
	internal static Hook OnGameLoad_Hook = new Hook(
		typeof(OnGameLoadManager).GetMethod("Run", BindingFlags.NonPublic | BindingFlags.Static),
		(Action orig) => {
			orig();
			foreach (Mod mod in LoadedMods) {
				mod.RegisterContent();
			}
		},
		new HookConfig() {
			ID = "RL2.API::RegisterContent",
			After = ["RL2.ModLoader.ModLoader::OnGameLoad"] 
		}
	);
}