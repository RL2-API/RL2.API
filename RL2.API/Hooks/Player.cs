using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using UnityEngine;
using Mono.Cecil.Cil;

namespace RL2.API;

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
	/// Handles calling <seealso cref="Player.PostUpdateStats"/>
	/// </summary>
	internal static Hook PostUpdateStats_Hook = new Hook(
		typeof(PlayerController).GetMethod("InitializeAllMods", BindingFlags.Public | BindingFlags.Instance),
		(Action<PlayerController, bool, bool> orig, PlayerController self, bool resetHP, bool resetMP) => {
			orig(self, resetHP, resetMP);
			Player.PostUpdateStats_Invoke(self);
		},
		new HookConfig() {
			ID = "RL2.API::PostUpdateStats"
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