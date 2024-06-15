using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RL2.ModLoader.APIEndpoints;

/// <summary>
/// 
/// </summary>
public class ModSystemEndpoints
{
	/// <summary>
	/// 
	/// </summary>
	public static GameManager GameManagerInstance;

	/// <summary>
	/// 
	/// </summary>
	internal static Hook AttachModSystemInstances = new Hook(
		typeof(GameManager).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance),
		(Action<GameManager> orig, GameManager self) => {
			orig(self);
			if (GameManagerInstance != null) {
				return;
			}
			GameManagerInstance = self;
			foreach (Mod mod in RL2API.LoadedMods) {
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
	/// 
	/// </summary>
	internal static Hook ModifyCharacterRandomization = new Hook(
		typeof(CharacterCreator).GetMethod("ApplyRandomizeKitTrait", BindingFlags.Public | BindingFlags.Static),
		(Action<CharacterData, bool, bool, bool> orig, CharacterData charData, bool randomizeSpell, bool excludeCurrentAbilities, bool useLineageSeed) => {
			orig(charData, randomizeSpell, excludeCurrentAbilities, useLineageSeed);
			foreach (ModSystem modSystem in GameManagerInstance!.GetComponents<ModSystem>()) {
				modSystem.ModifyCharacterRandomization(charData);
			}
		}
	);

	/// <summary>
	/// Handles calling <see cref="ModSystem.ModifyGeneratedCharacter(CharacterData)"/><br/>
	/// </summary>
	/// <remarks>Currently broken; DO NOT USE</remarks>
	internal static ILHook ModifyGeneratedCharacter = new ILHook(
		typeof(LineageWindowController).GetMethod("CreateRandomCharacters", BindingFlags.NonPublic | BindingFlags.Instance),
		(ILContext il) => {
			ILCursor ilCursor = new ILCursor(il).Goto(0);

			ilCursor.GotoNext(
				MoveType.Before,
				i => i.MatchLdloc(4),
				i => i.MatchLdarg(0),
				i => i.MatchLdfld<LineageWindowController>("m_characterDataArray"),
				i => i.MatchLdlen(),
				i => i.MatchConvI4(),
				i => i.MatchLdcI4(1),
				i => i.MatchSub()
			);
			
			ilCursor.Emit(OpCodes.Ldarg, 0);
			ilCursor.Emit(OpCodes.Ldfld, typeof(LineageWindowController).GetField("m_characterDataArray", BindingFlags.NonPublic | BindingFlags.Instance));
			ilCursor.Emit(OpCodes.Ldloc, 4);
			ilCursor.Emit(OpCodes.Ldelem_Ref);
			ilCursor.EmitDelegate((CharacterData characterData) => {
				Mod.Log("Worked... maybe");
				foreach (ModSystem modSystem in GameManagerInstance!.GetComponents<ModSystem>()) {
					modSystem.ModifyCharacterRandomization(characterData);
				}
			});
			ilCursor.EmitDelegate(() => { Debug.Log("Huh"); });
		}
	);

	/// <summary>
	/// 
	/// </summary>
	public static MapController MapControllerInstance;

	/// <summary>
	/// 
	/// </summary>
	internal static Hook SetMapControllerInstance = new Hook(
		typeof(MapController).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance),
		(Action<MapController> orig, MapController self) => {
			orig(self);
			if (MapControllerInstance != null) {
				return;
			}
			MapControllerInstance = self;
		}
	);

	/// <summary>
	/// 
	/// </summary>
	public static Dictionary<string, GameObject> TextureHashToPrefab = new Dictionary<string, GameObject>();

	/// <summary>
	/// 
	/// </summary>
	internal static Hook ModifyRoomIcon = new Hook(
		typeof(MapController).GetMethod("GetSpecialIconPrefab", BindingFlags.Public | BindingFlags.Static),
		(Func<GridPointManager, bool, bool, GameObject> orig, GridPointManager roomToCheck, bool getUsed, bool isMergeRoom) => {
			Texture2D? modMapIconTexture = null;
			foreach (ModSystem modSystem in GameManagerInstance.GetComponents<ModSystem>()) {
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
				GameObject modMapIconObject = UnityEngine.Object.Instantiate((GameObject)typeof(MapController).GetField("m_specialRoomIconPrefab", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(MapControllerInstance));
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
}