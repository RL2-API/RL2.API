using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using UnityEngine;

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

	internal static Hook ActualResolve = new Hook(
		typeof(PlayerController).GetProperty("ActualResolve", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(Func<PlayerController, float> orig, PlayerController self) => {
			float MinimumResolve = 0f;
			SoulShopObj soulShopObj = SaveManager.ModeSaveData.GetSoulShopObj(SoulShopType.MinimumResolveBlock);
			if (!soulShopObj.IsNativeNull()) {
				MinimumResolve += soulShopObj.CurrentStatGain;
			}

			MinimumResolve += EquipmentManager.Get_EquipmentSet_BonusTypeStatGain(EquipmentSetBonusType.MinimumResolve);
			float BaseResolve = self.BaseResolve;
			if (ChallengeManager.IsInChallenge) {
				BaseResolve = 2.5f;
			}

			return Mathf.Clamp((BaseResolve + self.ResolveAdd + StatBonuses.Resolve) * (1f + self.ResolveMod + StatBonuses.ResolveMultiplier), MinimumResolve, float.MaxValue);
		}
	);
	
	
	internal static Hook ActualDexterity = new Hook(
		typeof(PlayerController).GetProperty("ActualDexterity", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(Func<PlayerController, float> orig, PlayerController self) => {
			float FullDexterity = Mathf.Clamp((self.BaseDexterity + self.DexterityAdd + self.DexterityTemporaryAdd + StatBonuses.Dextrity) * (1f + self.DexterityMod + self.DexterityTemporaryMod + StatBonuses.DextrityMultiplier), 0f, float.MaxValue);
			if (ChallengeManager.IsInChallenge) {
				return ChallengeManager.ApplyStatCap(FullDexterity, isDexterityOrFocus: true);
			}

			return FullDexterity;
		}
	);

	internal static Hook ActualCritChance = new Hook(
		typeof(PlayerController).GetProperty("ActualCritChance", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(Func<PlayerController, float> orig, PlayerController self) => {
			return Mathf.Clamp(self.CritChanceAdd + self.CritChanceTemporaryAdd + 0f + StatBonuses.CritChance, 0f, 100f);
		}
	);

	internal static Hook ActualCritDamage = new Hook(
		typeof(PlayerController).GetProperty("ActualCritDamage", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(Func<PlayerController, float> orig, PlayerController self) => {
			return Mathf.Clamp((self.BaseCritDamage + self.CritDamageAdd + self.CritDamageTemporaryAdd + StatBonuses.CritDamage) * StatBonuses.CritDamageMultiplier, 0f, float.MaxValue);
		}
	);

	internal static Hook ActualFocus = new Hook(
		typeof(PlayerController).GetProperty("ActualFocus", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(Func<PlayerController, float> orig, PlayerController self) => {
			float FullFocus = Mathf.Clamp((self.BaseFocus + self.FocusAdd + self.FocusTemporaryAdd + StatBonuses.Focus) * (1f + self.FocusMod + self.FocusTemporaryMod + StatBonuses.FocusMultiplier), 0f, float.MaxValue);
			if (ChallengeManager.IsInChallenge) {
				return ChallengeManager.ApplyStatCap(FullFocus, isDexterityOrFocus: true);
			}

			return FullFocus;
		}
	);

	internal static Hook ActualMagicCritChance = new Hook(
		typeof(PlayerController).GetProperty("ActualMagicCritChance", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(Func<PlayerController, float> orig, PlayerController self) => {
			return Mathf.Clamp(self.MagicCritChanceAdd + self.MagicCritChanceTemporaryAdd + 0f + StatBonuses.MagicCritChance, 0f, 1f);
		}
	);

	internal static Hook ActualMagicCritDamage = new Hook(
		typeof(PlayerController).GetProperty("ActualMagicCritDamage", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(Func<PlayerController, float> orig, PlayerController self) => {
			return Mathf.Clamp((self.BaseMagicCritDamage + self.MagicCritDamageAdd + self.MagicCritDamageTemporaryAdd + StatBonuses.MagicCritDamage) * StatBonuses.MagicCritDamageMultiplier, 0f, float.MaxValue);
		}
	);

	internal static Hook ActualArmor = new Hook(
		typeof(PlayerController).GetProperty("ActualArmor", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(Func<PlayerController, int> orig, PlayerController self) => {
			return Mathf.Clamp((int)((self.BaseArmor + self.ArmorAdds + StatBonuses.Armor) * StatBonuses.ArmorMultiplier), 0, int.MaxValue);
		}
	);

	internal static Hook ActualStrength = new Hook(
		typeof(PlayerController).GetProperty("ActualStrength", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(Func<PlayerController, float> orig, PlayerController self) => {
			float FullStrength = Mathf.Clamp((self.BaseStrength + self.StrengthAdd + self.StrengthTemporaryAdd + StatBonuses.Strength) * (1f + self.StrengthMod + self.StrengthTemporaryMod + StatBonuses.StrengthMultiplier), 0f, float.MaxValue);
			if (ChallengeManager.IsInChallenge) {
				FullStrength = ChallengeManager.ApplyStatCap(FullStrength);
				FullStrength *= 1f + ChallengeManager.GetActiveHandicapMod();
			}

			return FullStrength;
		}
	);

	internal static Hook ActualMagic = new Hook(
		typeof(PlayerController).GetProperty("ActualMagic", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(Func<PlayerController, float> orig, PlayerController self) => {
			float FullMagic = Mathf.Clamp((self.BaseMagic + self.MagicAdd + self.MagicTemporaryAdd + StatBonuses.Intelligence) * (1f + self.MagicMod + self.MagicTemporaryMod + StatBonuses.IntelligenceMultiplier), 0f, float.MaxValue);
			if (ChallengeManager.IsInChallenge) {
				FullMagic = ChallengeManager.ApplyStatCap(FullMagic);
				FullMagic *= 1f + ChallengeManager.GetActiveHandicapMod();
			}

			return FullMagic;
		}
	);

	internal static Hook ActualVitality = new Hook(
		typeof(PlayerController).GetProperty("ActualVitality", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(Func<PlayerController, int> orig, PlayerController self) => {
			int FullVitality = self.BaseVitality + self.VitalityAdd;
			FullVitality = Mathf.CeilToInt(FullVitality * (1f + self.VitalityMod));
			if (ChallengeManager.IsInChallenge) {
				FullVitality = (int)ChallengeManager.ApplyStatCap(FullVitality);
				FullVitality = Mathf.CeilToInt(FullVitality * (1f + ChallengeManager.GetActiveHandicapMod()));
			}

			return FullVitality;
		}
	);

	internal static Hook ActualMaxHealth = new Hook(
		typeof(PlayerController).GetProperty("ActualMaxHealth", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(Func<PlayerController, int> orig, PlayerController self) => {
			if (TraitManager.IsTraitActive(TraitType.OneHitDeath)) {
				return 1;
			}

			int classModdedMaxHealth = self.ClassModdedMaxHealth;
			float traitMaxHealthMod = self.TraitMaxHealthMod;
			float relicMaxHealthMod = self.RelicMaxHealthMod;
			float maxHealthMod = self.MaxHealthMod;
			float HealthMultiplier = 1f + traitMaxHealthMod + relicMaxHealthMod + maxHealthMod + StatBonuses.HealthMultiplier;
			float ResolvePenalty = Mathf.Clamp(1f - self.ActualResolve, 0f, 1f);
			int BonusHealth = 150;

			return Mathf.Clamp(Mathf.CeilToInt((classModdedMaxHealth + BonusHealth + StatBonuses.Health) * (1f - ResolvePenalty) * HealthMultiplier), 1, int.MaxValue);
		}
	);

	internal static Hook ActualMaxMana = new Hook(
		typeof(PlayerController).GetProperty("ActualMaxMana", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(Func<PlayerController, int> orig, PlayerController self) => {
			return Mathf.Clamp(Mathf.CeilToInt((self.ClassModdedMaxMana * (1f + self.TraitMaxManaMod)) + self.PostModMaxManaAdd + StatBonuses.Mana) * StatBonuses.ManaMultiplier, 1, int.MaxValue);
		}
	);

	internal static Hook ActualRuneWeight = new Hook(
		typeof(PlayerController).GetProperty("ActualRuneWeight", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(Func<PlayerController, int> orig, PlayerController self) => {
			return self.BaseRuneWeight + self.RuneWeightAdds + StatBonuses.RuneWeight;
		}
	);

	internal static Hook ActualAllowedEquipmentWeight = new Hook(
		typeof(PlayerController).GetProperty("ActualAllowedEquipmentWeight", BindingFlags.Public | BindingFlags.Instance).GetGetMethod(),
		(Func<PlayerController, int> orig, PlayerController self) => {
			return self.BaseAllowedEquipmentWeight + self.AllowedEquipmentWeightAdds + StatBonuses.EquipmentWeight;
		}
	);
}