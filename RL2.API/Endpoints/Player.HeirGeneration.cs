using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;

namespace RL2.API;

public static partial class Player
{
	/// <summary>
	/// Contains endpoints for altering heir generation
	/// </summary>
	public static class HeirGeneration
	{
		internal static IDetour[] Hooks = [
			ModifyCharacterData.Hook,
			ModifyCharacterRandomization.Hook,
			ModifyCharacterLook.Hook,
		];

		/// <summary>
		/// Used to modify character data after character randomization by either the Contrarian trait or by the use of the Transmogrifier<br></br> 
		/// Ran at the end of <see cref="CharacterCreator.ApplyRandomizeKitTrait"/>
		/// </summary>
		public static class ModifyCharacterRandomization
		{
			/// <inheritdoc cref="ModifyCharacterRandomization"/>
			/// <param name="data">The randomized heirs CharacterData</param>
			public delegate void Definition(CharacterData data);

			/// <inheritdoc cref="Definition"/>
			public static event Definition? Event;

			internal static Hook Hook = new Hook(
				typeof(CharacterCreator).GetMethod("ApplyRandomizeKitTrait", BindingFlags.Public | BindingFlags.Static),
				Method,
				new HookConfig() {
					ID = "RL2.API::Player.HeirGeneration.ModifyCharacterRandomization",
					ManualApply = true,
				}
			);

			internal static void Method(Action<CharacterData, bool, bool, bool> orig, CharacterData charData, bool randomizeSpell, bool excludeCurrentAbilities, bool useLineageSeed) {
				orig(charData, randomizeSpell, excludeCurrentAbilities, useLineageSeed);
				Event?.Invoke(charData);
			}
		}

		/// <summary>
		/// Used to modify character data of generated heirs
		/// </summary>
		public static class ModifyCharacterData
		{
			/// <inheritdoc cref="ModifyCharacterData"/>
			/// <param name="data">The generated heirs <see cref="CharacterData" /></param>
			/// <param name="classLocked">Whether the heirs class was locked by a Soul Shop ugrade</param>
			/// <param name="spellLocked">Whether the heirs spell was locked by a Soul Shop ugrade</param>
			public delegate void Definition(CharacterData data, bool classLocked, bool spellLocked);

			/// <inheritdoc cref="Definition"/>
			public static event Definition? Event;

			internal static ILHook Hook = new ILHook(
				typeof(LineageWindowController).GetMethod("CreateRandomCharacters", BindingFlags.NonPublic | BindingFlags.Instance),
				Method,
				new ILHookConfig() {
					ID = "RL2.API::Player.HeirGeneration.ModifyCharacterData",
					ManualApply = true
				}
			);

			static void Method(ILContext il) {
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
				ilCursor.EmitDelegate(FinalCall);

				static void FinalCall(CharacterData characterData, int index, CharacterData[] array) {
					bool classLocked = index == array.Length - 1 && SaveManager.ModeSaveData.GetSoulShopObj(SoulShopType.ChooseYourClass).CurrentEquippedLevel > 0;
					bool spellLocked = index == array.Length - 1 && SaveManager.ModeSaveData.GetSoulShopObj(SoulShopType.ChooseYourSpell).CurrentEquippedLevel > 0;
					Event?.Invoke(characterData, classLocked, spellLocked);
				}
			}
		}

		/// <summary>
		/// Used to modify look data of generated heirs
		/// </summary>
		public static class ModifyCharacterLook
		{
			/// <inheritdoc cref="ModifyCharacterLook"/>
			/// <param name="lookData">The generated heirs <see cref="PlayerLookController" /></param>
			/// <param name="data">The generated heirs <see cref="CharacterData" /></param>
			public delegate void Definition(PlayerLookController lookData, CharacterData data);

			/// <inheritdoc cref="Definition"/>
			public static event Definition? Event;

			internal static ILHook Hook = new ILHook(
				typeof(LineageWindowController).GetMethod("CreateRandomCharacters", BindingFlags.NonPublic | BindingFlags.Instance),
				Method,
				new ILHookConfig() {
					ID = "RL2.API::Player.HeirGeneration.ModifyCharacterLook",
					ManualApply = true,
				}
			);

			internal static void Method(ILContext il) {
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
					Event?.Invoke(lookData, characterData.Clone());
				});
			}
		}
	}
}