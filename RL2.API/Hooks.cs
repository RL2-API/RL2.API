using System.Reflection;
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
		..Burdens.Hooks,
		..SoulShop.Hooks,
		new Hook(
			typeof(ProfileSlotButton).GetMethod(nameof(ProfileSlotButton.ExecuteButton), BindingFlags.Public | BindingFlags.Instance),
			(System.Action<ProfileSlotButton> orig, ProfileSlotButton self) => {
				PreviousProfile = SaveManager.CurrentProfile;
				orig(self);
			},
			new HookConfig() {
				ID = "RL2.API::GetPreviousProfileSlot",
				ManualApply = true,
			}
		),
	];

	internal static int PreviousProfile;

	internal static void Apply() {
		foreach (IDetour detour in List)
			try { detour.Apply(); }
			catch (System.Exception ex) { RL2API.Log($"Failed to apply detour {detour}: {ex}"); }
	}

	internal static void Undo() {
		foreach (IDetour detour in List) detour.Undo();
	}
}