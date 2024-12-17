using MonoMod.RuntimeDetour;
using System;
using System.Reflection;

namespace RL2.API;

public partial class RL2API 
{
	internal static bool Loaded = false;

	internal static Hook OnGameLoad_Hook = new Hook(
		typeof(MainMenuWindowController).GetMethod("OnOpen", BindingFlags.NonPublic | BindingFlags.Instance),
		(Action<MainMenuWindowController> orig, MainMenuWindowController self) => {
			orig(self);
			if (!Loaded) {
				foreach (Mod mod in LoadedMods) {
					mod.RegisterContent();
				}
			}
		},
		new HookConfig() {
			ID = "RL2.API::RegisterContent",
			After = ["RL2.ModLoader.ModLoader::OnGameLoad"] 
		}
	);
}