using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using UnityEngine;
using RL2.ModLoader.API;
using Mono.Cecil.Cil;

namespace RL2.ModLoader;

public partial class RL2API
{
	/// <summary>
	/// Handles calling <see cref="Player.OnSpawn"/>
	/// </summary>
	internal static Hook OnSpawn_Player = new Hook(
		typeof(PlayerController).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance),
		(Action<PlayerController> orig, PlayerController self) => {
			orig(self);

			Player.OnSpawn_Invoke(self);
		}
	);

	#region Death
	/// <summary>
	///  Handles calling <seealso cref="Player.PreKill"/>
	/// </summary>
	internal static Hook PreKill_Player = new Hook(
		typeof(PlayerController).GetMethod("KillCharacter", BindingFlags.Public | BindingFlags.Instance),
		(Action<PlayerController, GameObject, bool> orig, PlayerController self, GameObject killer, bool broadcastEvents) => {
			if (self.IsDead) {
				return;
			}

			bool PreventDeath = !Player.PreKill_Invoke(self, killer);
			if (PreventDeath) {
				self.SetHealth(1f, additive: false, runEvents: true);
				return;
			}
			orig(self, killer, broadcastEvents);
		}
	);

	/// <summary>
	/// Handles calling <seealso cref="Player.OnKill"/>
	/// </summary>
	internal static Hook OnKill_Player = new Hook(
		typeof(PlayerController).GetMethod("KillCharacter", BindingFlags.Public | BindingFlags.Instance),
		(Action<PlayerController, GameObject, bool> orig, PlayerController self, GameObject killer, bool broadcastEvents) => {
			orig(self, killer, broadcastEvents);
			Player.OnKill_Invoke(self, killer);
		}
	);
	#endregion

	#region Stats
	/// <summary>
	/// Handles modifying Resolve
	/// </summary>
	internal static ILHook ActualResolve = new ILHook(
		typeof(PlayerController).GetProperty("ActualResolve", BindingFlags.Public | BindingFlags.Instance).GetMethod,
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdloc(2),
				i => i.MatchLdarg(0),
				i => i.MatchCall<PlayerController>("get_ResolveAdd"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((float vanillaResolve) => {
				Player.Stats.ResolveFlat_Invoke(ref vanillaResolve);
				return vanillaResolve;
			});

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdcR4(1),
				i => i.MatchLdarg(0),
				i => i.MatchCall<PlayerController>("get_ResolveMod"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((float vanillaResolveMultiplier) => {
				Player.Stats.ResolveMultiplier_Invoke(ref vanillaResolveMultiplier);
				return vanillaResolveMultiplier;
			});
		}
	);

	/// <summary>
	/// Handles modifying Dexterity
	/// </summary>
	internal static ILHook ActualDexterity = new ILHook(
		typeof(PlayerController).GetProperty("ActualDexterity", BindingFlags.Public | BindingFlags.Instance).GetMethod,
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCall<PlayerController>("get_DexterityTemporaryAdd"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((float vanillaDexterity) => {
				Player.Stats.DexterityFlat_Invoke(ref vanillaDexterity);
				return vanillaDexterity;
			});

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCall<PlayerController>("get_DexterityTemporaryMod"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((float vanillaDexterityMultiplier) => {
				Player.Stats.DexterityMultiplier_Invoke(ref vanillaDexterityMultiplier);
				return vanillaDexterityMultiplier;
			});
		}
	);

	/// <summary>
	/// Handles modifying CritChance
	/// </summary>
	internal static ILHook ActualCritChance = new ILHook(
		typeof(PlayerController).GetProperty("ActualCritChance", BindingFlags.Public | BindingFlags.Instance).GetMethod,
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCall<PlayerController>("get_CritChanceTemporaryAdd"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((float vanillaCritChance) => {
				Player.Stats.CritChance_Invoke(ref vanillaCritChance);
				return vanillaCritChance;
			});
		}
	);

	/// <summary>
	/// Handles modifying CritDamage
	/// </summary>
	internal static ILHook ActualCritDamage = new ILHook(
		typeof(PlayerController).GetProperty("ActualCritDamage", BindingFlags.Public | BindingFlags.Instance).GetMethod,
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCallvirt<PlayerController>("get_CritDamageTemporaryAdd"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((float vanillaCritDamage) => {
				Player.Stats.CritDamage_Invoke(ref vanillaCritDamage);
				return vanillaCritDamage;
			});
		}
	);

	/// <summary>
	/// Handles modifying Focus
	/// </summary>
	internal static ILHook ActualFocus = new ILHook(
		typeof(PlayerController).GetProperty("ActualFocus", BindingFlags.Public | BindingFlags.Instance).GetMethod,
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCallvirt<PlayerController>("get_FocusTemporaryAdd"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((float vanillaFocus) => {
				Player.Stats.FocusFlat_Invoke(ref vanillaFocus);
				return vanillaFocus;
			});

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCall<PlayerController>("get_FocusTemporaryMod"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((float vanillaFocusMultiplier) => {
				Player.Stats.FocusMultiplier_Invoke(ref vanillaFocusMultiplier);
				return vanillaFocusMultiplier;
			});
		}
	);

	/// <summary>
	/// Handles modifying MagicCritChance
	/// </summary>
	internal static ILHook ActualMagicCritChance = new ILHook(
		typeof(PlayerController).GetProperty("ActualMagicCritChance", BindingFlags.Public | BindingFlags.Instance).GetMethod,
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCall<PlayerController>("get_MagicCritChanceTemporaryAdd"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((float vanillaMagicCritChance) => {
				Player.Stats.MagicCritChance_Invoke(ref vanillaMagicCritChance);
				return vanillaMagicCritChance;
			});
		}
	);

	/// <summary>
	/// Handles modifying MagicCritDamage
	/// </summary>
	internal static ILHook ActualMagicCritDamage = new ILHook(
		typeof(PlayerController).GetProperty("ActualMagicCritDamage", BindingFlags.Public | BindingFlags.Instance).GetMethod,
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCall<PlayerController>("get_MagicCritDamageTemporaryAdd"),
				i => i.MatchAdd()
			);


			cursor.EmitDelegate((float vanillaMagicCritDamage) => {
				Player.Stats.MagicCritDamage_Invoke(ref vanillaMagicCritDamage);
				return vanillaMagicCritDamage;
			});
		}
	);

	/// <summary>
	/// Handles modifying Armor
	/// </summary>
	internal static ILHook ArmorAdds = new ILHook(
		typeof(PlayerController).GetMethod("InitializeArmorMods", BindingFlags.Public | BindingFlags.Instance),
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchLdloc(0)
			);

			cursor.EmitDelegate((int vanillaFlatArmor) => {
				Player.Stats.ArmorFlat_Invoke(ref vanillaFlatArmor);
				return vanillaFlatArmor;
			});

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdloc(1)
			);

			cursor.EmitDelegate((float vanillaArmorMultiplier) => {
				Player.Stats.ArmorMultiplier_Invoke(ref vanillaArmorMultiplier);
				return vanillaArmorMultiplier;
			});
		}
	);

	/// <summary>
	/// Handles modifying Strength
	/// </summary>
	internal static ILHook ActualStrength = new ILHook(
		typeof(BaseCharacterController).GetProperty("ActualStrength", BindingFlags.Public | BindingFlags.Instance).GetMethod,
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.Before,
				i => i.MatchConvR4(),
				i => i.MatchAdd(),
				i => i.MatchLdcR4(1)
			);

			cursor.Index += 2;
			cursor.Emit(OpCodes.Ldarg_0);
			cursor.EmitDelegate((float vanillaFlatStrength, BaseCharacterController self) => {
				if (self is not PlayerController) {
					return vanillaFlatStrength;
				}

				Player.Stats.StrengthFlat_Invoke(ref vanillaFlatStrength);
				return vanillaFlatStrength;
			});

			cursor.GotoNext(
				MoveType.Before,
				i => i.MatchAdd(),
				i => i.MatchMul()
			);

			cursor.Index++;
			cursor.Emit(OpCodes.Ldarg_0);
			cursor.EmitDelegate((float vanillaStrengthMultiplier, BaseCharacterController self) => {
				if (self is not PlayerController) {
					return vanillaStrengthMultiplier;
				}

				Player.Stats.StrengthMultiplier_Invoke(ref vanillaStrengthMultiplier);
				return vanillaStrengthMultiplier;
			});
		}
	);

	/// <summary>
	/// Handles modifying Magic
	/// </summary>
	internal static ILHook ActualMagic = new ILHook(
		typeof(BaseCharacterController).GetProperty("ActualMagic", BindingFlags.Public | BindingFlags.Instance).GetMethod,
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.Before,
				i => i.MatchConvR4(),
				i => i.MatchAdd(),
				i => i.MatchLdcR4(1)
			);

			cursor.Index += 2;
			cursor.Emit(OpCodes.Ldarg_0);
			cursor.EmitDelegate((float vanillaFlatIntelligence, BaseCharacterController self) => {
				if (self is not PlayerController) {
					return vanillaFlatIntelligence;
				}

				Player.Stats.IntelligenceFlat_Invoke(ref vanillaFlatIntelligence);
				return vanillaFlatIntelligence;
			});

			cursor.GotoNext(
				MoveType.Before,
				i => i.MatchAdd(),
				i => i.MatchMul()
			);

			cursor.Index++;
			cursor.Emit(OpCodes.Ldarg_0);
			cursor.EmitDelegate((float vanillaIntelligenceMultiplier, BaseCharacterController self) => {
				if (self is not PlayerController) {
					return vanillaIntelligenceMultiplier;
				}

				Player.Stats.IntelligenceMultiplier_Invoke(ref vanillaIntelligenceMultiplier);
				return vanillaIntelligenceMultiplier;
			});
		}
	);

	/// <summary>
	/// Handles modifying Vitality
	/// </summary>
	internal static ILHook ActualVitality = new ILHook(
		typeof(PlayerController).GetProperty("ActualVitality", BindingFlags.Public | BindingFlags.Instance).GetMethod,
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCall<PlayerController>("get_VitalityAdd"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((int vanillaVitality) => {
				Player.Stats.VitalityFlat_Invoke(ref vanillaVitality);
				return vanillaVitality;
			});

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCall<PlayerController>("get_VitalityMod"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((float vanillaVitalityMultiplier) => {
				Player.Stats.VitalityMultiplier_Invoke(ref vanillaVitalityMultiplier);
				return vanillaVitalityMultiplier;
			});
		}
	);

	/// <summary>
	/// Handles modifying MaxHealth
	/// </summary>
	internal static ILHook ActualMaxHealth = new ILHook(
		typeof(PlayerController).GetProperty("ActualMaxHealth", BindingFlags.Public | BindingFlags.Instance).GetMethod,
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchAdd(),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((float vanillaMaxHealthMultiplier) => {
				Player.Stats.MaxHealthMultiplier_Invoke(ref vanillaMaxHealthMultiplier);
				return vanillaMaxHealthMultiplier;
			});

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdcI4(150)
			);

			cursor.EmitDelegate((int vanillaMaxHealth) => {
				Player.Stats.MaxHealthFlat_Invoke(ref vanillaMaxHealth);
				return vanillaMaxHealth;
			});
		}
	);

	/// <summary>
	/// Handles modifying MaxMana
	/// </summary>
	internal static ILHook ActualMaxMana = new ILHook(
		typeof(PlayerController).GetProperty("ActualMaxMana", BindingFlags.Public | BindingFlags.Instance).GetMethod,
		(ILContext il) => {
			ILCursor cursor = new ILCursor(il);

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCall<PlayerController>("get_TraitMaxManaMod"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((float vanillaMaxMana) => {
				Player.Stats.MaxManaMultiplier_Invoke(ref vanillaMaxMana);
				return vanillaMaxMana;
			});

			cursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchCall<PlayerController>("get_PostModMaxManaAdd"),
				i => i.MatchAdd()
			);

			cursor.EmitDelegate((int vanillaMaxMana) => {
				Player.Stats.MaxManaFlat_Invoke(ref vanillaMaxMana);
				return vanillaMaxMana;});
		}
	);

	/// <summary>
	/// Handles modifying RuneWeight
	/// </summary>
	internal static Hook ActualRuneWeight = new Hook(
		typeof(PlayerController).GetProperty("ActualRuneWeight", BindingFlags.Public | BindingFlags.Instance).GetMethod,
		(Func<PlayerController, int> orig, PlayerController self) => {
			int vanillaRuneWeight = orig(self);
			Player.Stats.EquipmentWeight_Invoke(ref vanillaRuneWeight);
			return vanillaRuneWeight;
		}
	);

	/// <summary>
	/// Handles modifying EquipmentWeight
	/// </summary>
	internal static Hook ActualAllowedEquipmentWeight = new Hook(
		typeof(PlayerController).GetProperty("ActualAllowedEquipmentWeight", BindingFlags.Public | BindingFlags.Instance).GetMethod,
		(Func<PlayerController, int> orig, PlayerController self) => {
			int vanillaEquipmentWeight = orig(self);
			Player.Stats.EquipmentWeight_Invoke(ref vanillaEquipmentWeight);
			return vanillaEquipmentWeight;
		}
	);

	#endregion

	#region Character generation
	/// <summary>
	/// Handles calling <see cref="Player.HeirGeneration.ModifyCharacterRandomization"/>
	/// </summary>
	internal static Hook ModifyCharacterRandomization = new Hook(
		typeof(CharacterCreator).GetMethod("ApplyRandomizeKitTrait", BindingFlags.Public | BindingFlags.Static),
		(Action<CharacterData, bool, bool, bool> orig, CharacterData charData, bool randomizeSpell, bool excludeCurrentAbilities, bool useLineageSeed) => {
			orig(charData, randomizeSpell, excludeCurrentAbilities, useLineageSeed);
			Player.HeirGeneration.ModifyCharacterRandomization_Invoke(charData);
		}
	);


	/// <summary>
	/// Handles calling <see cref="Player.HeirGeneration.ModifyCharacterData"/>
	/// </summary>
	internal static ILHook ModifyCharacterData = new ILHook(
		typeof(LineageWindowController).GetMethod("CreateRandomCharacters", BindingFlags.NonPublic | BindingFlags.Instance),
		(ILContext il) => {
			ILCursor ilCursor = new ILCursor(il);
			ILLabel endpoint = ilCursor.DefineLabel();

			// Repoint the breaks to point to the coorrect place
			ilCursor.GotoNext(
				MoveType.After,
				i => i.MatchLdsfld<SaveManager>(nameof(SaveManager.PlayerSaveData)),
				i => i.MatchLdfld<PlayerSaveData>(nameof(PlayerSaveData.PlayerDiedToZombie))
			);

			ilCursor.Remove();
			ilCursor.Emit(OpCodes.Brfalse, endpoint);

			ilCursor.GotoNext(
				MoveType.After,
				i => i.MatchLdsfld<SaveManager>(nameof(SaveManager.PlayerSaveData)),
				i => i.MatchLdfld<PlayerSaveData>(nameof(PlayerSaveData.TimesRolledLineage))
			);

			ilCursor.Remove();
			ilCursor.Emit(OpCodes.Brtrue, endpoint);

			ilCursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchLdfld<LineageWindowController>("m_characterDataArray"),
				i => i.MatchLdloc(4),
				i => i.MatchLdelemRef(),
				i => i.MatchLdcI4(0),
				i => i.MatchStfld<CharacterData>(nameof(CharacterData.AntiqueTwoOwned))
			);

			ilCursor.Remove();
			ilCursor.Emit(OpCodes.Br, endpoint);

			ilCursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchLdfld<LineageWindowController>("m_characterDataArray"),
				i => i.MatchLdloc(4),
				i => i.MatchLdelemRef(),
				i => i.MatchLdcI4(0),
				i => i.MatchStfld<CharacterData>(nameof(CharacterData.AntiqueOneOwned))
			);

			ilCursor.Remove();
			ilCursor.Emit(OpCodes.Br_S, endpoint);

			ilCursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchLdfld<LineageWindowController>("m_characterDataArray"),
				i => i.MatchLdloc(4),
				i => i.MatchLdelemRef(),
				i => i.MatchLdcI4(0),
				i => i.MatchStfld<CharacterData>(nameof(CharacterData.AntiqueTwoOwned))
			);

			ilCursor.Remove();
			ilCursor.Emit(OpCodes.Br_S, endpoint);

			// Insert out own code to modify character data after it was created
			ilCursor.GotoNext(
				i => i.MatchLdloc(4),
				i => i.MatchLdarg(0),
				i => i.MatchLdfld<LineageWindowController>("m_characterDataArray"),
				i => i.MatchLdlen(),
				i => i.MatchConvI4(),
				i => i.MatchLdcI4(1),
				i => i.MatchSub()
			);

			ilCursor.MarkLabel(endpoint);
			ilCursor.Emit(OpCodes.Ldarg, 0);
			ilCursor.Emit(OpCodes.Ldfld, typeof(LineageWindowController).GetField("m_characterDataArray", BindingFlags.NonPublic | BindingFlags.Instance));
			ilCursor.Emit(OpCodes.Ldloc, 4);
			ilCursor.Emit(OpCodes.Ldelem_Ref);
			ilCursor.Emit(OpCodes.Ldloc, 4);
			ilCursor.Emit(OpCodes.Ldarg, 0);
			ilCursor.Emit(OpCodes.Ldfld, typeof(LineageWindowController).GetField("m_characterDataArray", BindingFlags.NonPublic | BindingFlags.Instance));
			ilCursor.EmitDelegate((CharacterData characterData, int index, CharacterData[] array) => {
				bool classLocked = index == array.Length - 1 && SaveManager.ModeSaveData.GetSoulShopObj(SoulShopType.ChooseYourClass).CurrentEquippedLevel > 0;
				bool spellLocked = index == array.Length - 1 && SaveManager.ModeSaveData.GetSoulShopObj(SoulShopType.ChooseYourSpell).CurrentEquippedLevel > 0;
				Player.HeirGeneration.ModifyCharacterData_Invoke(characterData, classLocked, spellLocked);
			});
		}
	);

	internal ILHook ModifyCharacterLook = new ILHook(
		typeof(LineageWindowController).GetMethod("CreateRandomCharacters", BindingFlags.NonPublic | BindingFlags.Instance),
		(ILContext il) => {
			ILCursor ilCursor = new ILCursor(il);

			ilCursor.GotoNext(
				MoveType.After,
				i => i.MatchLdarg(0),
				i => i.MatchLdfld<LineageWindowController>("m_playerModels"),
				i => i.MatchLdloc(4),
				i => i.MatchLdelemRef(),
				i => i.MatchCallvirt<UnityEngine.Component>("get_transform"),
				i => i.MatchLdloc(10),
				i => i.MatchCallvirt<UnityEngine.Transform>("set_localScale")
			);

			ilCursor.Emit(OpCodes.Ldarg, 0);
			ilCursor.Emit(OpCodes.Ldfld, typeof(LineageWindowController).GetField("m_playerModels", BindingFlags.NonPublic | BindingFlags.Instance));
			ilCursor.Emit(OpCodes.Ldloc, 4);
			ilCursor.Emit(OpCodes.Ldelem_Ref);
			ilCursor.Emit(OpCodes.Ldarg, 0);
			ilCursor.Emit(OpCodes.Ldfld, typeof(LineageWindowController).GetField("m_characterDataArray", BindingFlags.NonPublic | BindingFlags.Instance));
			ilCursor.Emit(OpCodes.Ldloc, 4);
			ilCursor.Emit(OpCodes.Ldelem_Ref);
			ilCursor.EmitDelegate((PlayerLookController lookData, CharacterData characterData) => {
				Player.HeirGeneration.ModifyCharacterLook_Invoke(lookData, characterData.Clone());
			});
		}
	);
	#endregion

	#region Ability

	internal static Hook ModifyAbilityDataHook = new Hook(
		typeof(AbilityLibrary).GetMethod("GetAbility", BindingFlags.Public | BindingFlags.Static),
		(Func<AbilityType, BaseAbility_RL> orig, AbilityType type) => {
			BaseAbility_RL? ability = orig(type);
			if (ability == null) {
				return ability;
			}

			Player.Ability.ModifyData_Invoke(type, ability.AbilityData);

			return ability;
		}
	);

	#endregion
}