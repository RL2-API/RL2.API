using MonoMod.RuntimeDetour;
using System;
using System.Reflection;

namespace RL2.ModLoader;

public partial class ModLoader {

	/// <summary>
	/// Attaches a <see cref="Console"/> component to the <see cref="GameManager"/>
	/// </summary>
	internal static Hook ConsoleHook = new Hook(
		typeof(GameManager).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance),
		(Action<GameManager> orig, GameManager self) => {
			if (!GameManager.IsGameManagerInstantiated) {
				self.gameObject.AddComponent<Console>();
			}
			orig(self);
		}
	);
}