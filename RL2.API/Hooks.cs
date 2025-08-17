using MonoMod.RuntimeDetour;

namespace RL2.API;

internal static class Hooks
{
	internal static IDetour[] List = [
		..Player.Hooks,
		..Map.Hooks,
		..Enemy.Hooks,
		..Scars.Hooks,
		..Relics.Hooks,
		..Traits.Hooks,
	];

	internal static void Apply() {
		foreach (IDetour detour in List)
			try { detour.Apply(); }
			catch (System.Exception ex) { RL2API.Log($"Failed to apply detour {detour}: {ex}"); }
	}

	internal static void Undo() {
		foreach (IDetour detour in List) detour.Apply();
	}
}