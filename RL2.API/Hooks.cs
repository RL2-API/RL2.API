using MonoMod.RuntimeDetour;

namespace RL2.API;

internal static class Hooks
{
	internal static IDetour[] List = [
		..Player.Hooks,
		..Map.Hooks,
		..Enemy.Hooks,
	];

	internal static void Apply() {
		foreach (IDetour detour in List) detour.Apply();
	}

	internal static void Undo() {
		foreach (IDetour detour in List) detour.Apply();
	}
}