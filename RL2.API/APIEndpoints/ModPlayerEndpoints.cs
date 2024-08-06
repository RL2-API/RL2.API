using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace RL2.ModLoader;

public partial class RL2API
{
	/// <summary>
	/// Handles attaching ModPlayer objects to the player
	/// </summary>
	internal static Hook AttachModPLayerInstances = new Hook(
		typeof(PlayerController).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance)	,
		(Action<PlayerController> orig, PlayerController self) => {
			orig(self);
			foreach (Mod mod in LoadedMods) {
				foreach (Type modPlayer in mod.GetModTypes<ModPlayer>()) {
					ModPlayer modPlayerInstance = (ModPlayer)self.gameObject.AddComponent(modPlayer);
					if (modPlayerInstance.IsLoadingEnabled()) {
						modPlayerInstance.OnLoad();
						continue;
					}
					UnityEngine.Object.Destroy(modPlayerInstance);
				}
			}
		}
	);

	internal static PlayerStatBonuses StatBonuses = new PlayerStatBonuses();

	/// <summary>
	/// Handles calling <see cref="ModPlayer.ModifyStats"/>
	/// </summary>
	internal static Hook ModifyStats = new Hook(
		typeof(PlayerController).GetMethod("InitializeAllMods", BindingFlags.Public | BindingFlags.Instance),
		(Action<PlayerController, bool, bool> orig, PlayerController self, bool resetHP, bool resetMP) => {
			StatBonuses.Reset();
			foreach (ModPlayer modPlayer in self.gameObject.GetComponents<ModPlayer>()) {
				modPlayer.ModifyStats();
			}
			orig(self, resetHP, resetMP);
		}
	);

	/// <summary>
	/// Handles death prevention for players
	/// </summary>
	internal static Hook PreKill_Player = new Hook(
		typeof(PlayerController).GetMethod("KillCharacter", BindingFlags.Public | BindingFlags.Instance),
		(Action<PlayerController, GameObject, bool> orig, PlayerController self, GameObject killer, bool broadcastEvents) => {
			if (self.IsDead) {
				return;
			}

			bool PreventDeath = false;
			foreach (ModPlayer modPlayer in self.gameObject.GetComponents<ModPlayer>()) {
				if (!modPlayer.PreKill(killer)) {
					PreventDeath = true;
					break;
				}
			}
			if (PreventDeath) {
				self.SetHealth(1f, additive: false, runEvents: true);
				return;
			}
			orig(self, killer, broadcastEvents);
		}
	);

	/// <summary>
	/// Handles calling <seealso cref="ModPlayer.OnKill"/>
	/// </summary>
	internal static Hook OnKill_Player = new Hook(
		typeof(PlayerController).GetMethod("KillCharacter", BindingFlags.Public | BindingFlags.Instance),
		(Action<PlayerController, GameObject, bool> orig, PlayerController self, GameObject killer, bool broadcastEvents) => {
			orig(self, killer, broadcastEvents);
			foreach (ModPlayer modPlayer in self.gameObject.GetComponents<ModPlayer>()) {
				modPlayer.OnKill(killer);
			}
			StatBonuses.Reset();
		}
	);

	internal static ILHook ActualResolve = new ILHook(
		typeof(PlayerController).GetProperty("ActualResolve", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);
			
			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdloc(2),
				i => i.MatchLdarg(0),
				i => i.MatchCall<PlayerController>("get_ResolveAdd"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((float vanillaResolve) => vanillaResolve + StatBonuses.Resolve);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdcR4(1),
				i => i.MatchLdarg(0),
				i => i.MatchCall<PlayerController>("get_ResolveMod"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((float vanillaResolveMultiplier) => vanillaResolveMultiplier + StatBonuses.ResolveMultiplier);
		}
	);
	
	internal static ILHook ActualDexterity = new ILHook(
		typeof(PlayerController).GetProperty("ActualDexterity", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCall<PlayerController>("get_DexterityTemporaryAdd"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((float vanillaDexterity) => vanillaDexterity + StatBonuses.Dextrity);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCall<PlayerController>("get_DexterityTemporaryMod"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((float vanillaDexterityMultiplier) => vanillaDexterityMultiplier + StatBonuses.DextrityMultiplier);
		}
	);

	internal static ILHook ActualCritChance = new ILHook(
		typeof(PlayerController).GetProperty("ActualCritChance", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCall<PlayerController>("get_CritChanceTemporaryAdd"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((float vanillaCritChance) => vanillaCritChance + StatBonuses.CritChance);
		}
	);

	internal static ILHook ActualCritDamage = new ILHook(
		typeof(PlayerController).GetProperty("ActualCritDamage", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCallvirt<PlayerController>("get_CritDamageTemporaryAdd"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((float vanillaCritChance) => vanillaCritChance + StatBonuses.CritDamage);
		}
	);

	internal static ILHook ActualFocus = new ILHook(
		typeof(PlayerController).GetProperty("ActualFocus", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCallvirt<PlayerController>("get_FocusTemporaryAdd"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((float vanillaFocus) => vanillaFocus + StatBonuses.Focus);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCall<PlayerController>("get_FocusTemporaryMod"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((float vanillaFocusMultiplier) => vanillaFocusMultiplier + StatBonuses.FocusMultiplier);
		}
	);

	internal static ILHook ActualMagicCritChance = new ILHook(
		typeof(PlayerController).GetProperty("ActualMagicCritChance", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCall<PlayerController>("get_MagicCritChanceTemporaryAdd"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((float vanillaMagicCritChance) => vanillaMagicCritChance + StatBonuses.MagicCritChance);
		}
	);

	internal static ILHook ActualMagicCritDamage = new ILHook(
		typeof(PlayerController).GetProperty("ActualMagicCritDamage", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCall<PlayerController>("get_MagicCritDamageTemporaryAdd"),
				i => i.MatchAdd()
			);


			cursor.EmitDelegate((float vanillaMagicCritDamage) => vanillaMagicCritDamage + StatBonuses.MagicCritDamage);
		}
	);

	internal static ILHook ActualArmor = new ILHook(
		typeof(PlayerController).GetProperty("ActualArmor", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCall<PlayerController>("get_ArmorAdds"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((int vanillaArmor) => (int)((vanillaArmor + StatBonuses.Armor) * StatBonuses.ArmorMultiplier));
		}
	);
	
	internal static ILHook ActualStrength = new ILHook(
		typeof(PlayerController).GetProperty("ActualStrength", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0)
			);

			cursor.Remove();
			cursor.EmitDelegate((PlayerController player) => Mathf.Clamp((player.BaseStrength + player.StrengthAdd + player.StrengthTemporaryAdd + StatBonuses.Strength) * (1f + player.StrengthMod + player.StrengthTemporaryMod + StatBonuses.StrengthMultiplier), 0f, float.MaxValue));
		}
	);

	internal static ILHook ActualMagic = new ILHook(
		typeof(PlayerController).GetProperty("ActualMagic", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0)
			);

			cursor.Remove();
			cursor.EmitDelegate((PlayerController player) => Mathf.Clamp((player.BaseMagic + player.MagicAdd + player.MagicTemporaryAdd + StatBonuses.Intelligence) * (1f + player.MagicMod + player.MagicTemporaryMod + StatBonuses.IntelligenceMultiplier), 0f, float.MaxValue));
		}
	);

	internal static ILHook ActualVitality = new ILHook(
		typeof(PlayerController).GetProperty("ActualVitality", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCall<PlayerController>("get_VitalityAdd"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((int vanillaVitality) => vanillaVitality + StatBonuses.Vitality);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCall<PlayerController>("get_VitalityMod"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((float vanillaVitalityMultiplier) => vanillaVitalityMultiplier + StatBonuses.VitalityMultiplier);
		}
	);

	internal static ILHook ActualMaxHealth = new ILHook(
		typeof(PlayerController).GetProperty("ActualMaxHealth", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchAdd(),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((float vanillaMaxHealthMultiplier) => vanillaMaxHealthMultiplier + StatBonuses.HealthMultiplier);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdcI4(150)
			);

			cursor.EmitDelegate((int vanillaMaxHealth) => vanillaMaxHealth + StatBonuses.Health);
		}
	);

	internal static ILHook ActualMaxMana = new ILHook(
		typeof(PlayerController).GetProperty("ActualMaxMana", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCall<PlayerController>("get_PostModMaxManaAdd"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((int vanillaMaxMana) => (int)((vanillaMaxMana + StatBonuses.Mana) * StatBonuses.ManaMultiplier));
		}
	);

	internal static Hook ActualRuneWeight = new Hook(
		typeof(PlayerController).GetProperty("ActualRuneWeight", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(Func<PlayerController, int> orig, PlayerController self) => {
			return orig(self) + StatBonuses.RuneWeight;
		}
	);

	internal static Hook ActualAllowedEquipmentWeight = new Hook(
		typeof(PlayerController).GetProperty("ActualAllowedEquipmentWeight", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(Func<PlayerController, int> orig, PlayerController self) => {
			return orig(self) + StatBonuses.EquipmentWeight;
		}
	);
}