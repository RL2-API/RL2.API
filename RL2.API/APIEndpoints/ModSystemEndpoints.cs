using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RL2.ModLoader;

public partial class RL2API
{
	/// <summary>
	/// Handles attaching <see cref="ModSystem"/> instances
	/// </summary>
	internal static Hook AttachModSystemInstances = new Hook(
		typeof(GameManager).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance),
		(Action<GameManager> orig, GameManager self) => {
			orig(self);
			foreach (Mod mod in LoadedMods) {
				foreach (Type modSystemType in mod.GetModTypes<ModSystem>()) {
					ModSystem modSystem = (ModSystem)self.gameObject.AddComponent(modSystemType);
					if (!modSystem.IsLoadingEnabled()) {
						UnityEngine.Object.Destroy(modSystem);
						continue;
					}
					modSystem.OnLoad();
				}
			}
		}
	);

	/// <summary>
	/// Handles calling <see cref="ModSystem.ModifyCharacterRandomization(CharacterData)"/>
	/// </summary>
	internal static Hook ModifyCharacterRandomization = new Hook(
		typeof(CharacterCreator).GetMethod("ApplyRandomizeKitTrait", BindingFlags.Public | BindingFlags.Static),
		(Action<CharacterData, bool, bool, bool> orig, CharacterData charData, bool randomizeSpell, bool excludeCurrentAbilities, bool useLineageSeed) => {
			orig(charData, randomizeSpell, excludeCurrentAbilities, useLineageSeed);
			foreach (ModSystem modSystem in GameManager.Instance.GetComponents<ModSystem>()) {
				modSystem.ModifyCharacterRandomization(charData);
			}
		}
	);

	/// <summary>
	/// Handles calling <see cref="ModSystem.ModifyGeneratedCharacterData(CharacterData, bool, bool)"/><br/>
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
			ilCursor.EmitDelegate((CharacterData characterData) => {
				foreach (ModSystem modSystem in GameManager.Instance.GetComponents<ModSystem>()) {
					modSystem.ModifyGeneratedCharacterData(characterData, false, false);
				}
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
				foreach (ModSystem modSystem in GameManager.Instance.GetComponents<ModSystem>()) {
					modSystem.ModifyGeneratedCharacterLook(lookData, characterData.Clone());
				}
			});
		}
	);

	/// <summary>
	/// Stores map icons as prefabs, accessed by a 
	/// </summary>
	public static Dictionary<string, GameObject> TextureHashToPrefab = new Dictionary<string, GameObject>();

	/// <summary>
	/// Handles modifying the rooms icon on the map
	/// </summary>
	internal static Hook ModifyRoomIcon = new Hook(
		typeof(MapController).GetMethod("GetSpecialIconPrefab", BindingFlags.Public | BindingFlags.Static),
		(Func<GridPointManager, bool, bool, GameObject> orig, GridPointManager roomToCheck, bool getUsed, bool isMergeRoom) => {
			Texture2D? modMapIconTexture = null;
			foreach (ModSystem modSystem in GameManager.Instance.GetComponents<ModSystem>()) {
				if (modMapIconTexture != null) {
					break;
				}
				modMapIconTexture = modSystem.ModifyRoomIcon(roomToCheck, getUsed, isMergeRoom);
			}
			if (modMapIconTexture != null) {
				string textureHash = string.Concat(modMapIconTexture.GetPixels32());
				if (TextureHashToPrefab.ContainsKey(textureHash)) {
					return TextureHashToPrefab[textureHash];
				}
				GameObject modMapIconObject = UnityEngine.Object.Instantiate(MapController.m_instance.m_specialRoomIconPrefab);
				Sprite sprite = Sprite.Create(modMapIconTexture, new Rect(0f, 0f, modMapIconTexture.width, modMapIconTexture.height), new Vector2(.5f, .5f));
				modMapIconObject.GetComponent<SpriteRenderer>().sprite = sprite;
				UnityEngine.Object.DontDestroyOnLoad(modMapIconObject);
				modMapIconObject.SetActive(false);
				TextureHashToPrefab.Add(textureHash, modMapIconObject);
				return modMapIconObject;
			}
			return orig(roomToCheck, getUsed, isMergeRoom);
		}
	);

	internal static Hook ModifyAbilityDataHook = new Hook(
		typeof(AbilityLibrary).GetMethod("GetAbility", BindingFlags.Public | BindingFlags.Static),
		(Func<AbilityType, BaseAbility_RL> orig, AbilityType type) => {
			BaseAbility_RL? ability = orig(type);
			if (ability == null) {
				return ability;
			}

			foreach (ModSystem modSystem in GameManager.Instance.GetComponents<ModSystem>()) {
				modSystem.ModifyAbilityData(type, ability.AbilityData);
			}

			return ability;
		}
	);

	internal delegate bool EnemyClassDataDictionary_TryGetValue(EnemyTypeEnemyClassDataDictionary self, EnemyType key, out EnemyClassData data);

	internal static Hook ModifyEnemyClassDataHook = new Hook(
		typeof(EnemyTypeEnemyClassDataDictionary).GetMethod("TryGetValue", BindingFlags.Public | BindingFlags.Instance),
		ModifyEnemyClassDataMethod
	);

	internal static bool ModifyEnemyClassDataMethod(EnemyClassDataDictionary_TryGetValue orig, EnemyTypeEnemyClassDataDictionary self, EnemyType type, out EnemyClassData data) {
		bool found =  orig(self, type, out data);
		if (found) {
			foreach (ModSystem modSystem in GameManager.Instance.GetComponents<ModSystem>()) {
				foreach (EnemyRank rank in Enum.GetValues(typeof(EnemyRank))) {
					modSystem.ModifyEnemyData(type, rank, data.GetEnemyData(rank));
					modSystem.ModifyEnemyBehaviour(type, rank, data.GetAIScript(rank), data.GetLogicController());
				}
			}
		}

		return found;
	}
}
