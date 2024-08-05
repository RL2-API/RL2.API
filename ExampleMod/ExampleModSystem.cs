using RL2.ModLoader;
using UnityEngine;

namespace ExampleMod;

internal class ExampleModSystem : ModSystem
{
	public override void ModifyGeneratedCharacterData(CharacterData characterData, bool classLocked, bool spellLocked) {
		characterData.Name = "Test";
	}

	public override void ModifyGeneratedCharacterLook(PlayerLookController lookData, CharacterData characterData) {
		lookData.MouthGeo.material = LookLibrary.VampireFangsMaterial;
	}

	public override Texture2D? ModifyRoomIcon(GridPointManager roomToCheck, bool getUsed, bool isMergeRoom) {
		if (roomToCheck.RoomMetaData.IsSpecialRoom) {
			if (roomToCheck.RoomMetaData.SpecialRoomType == SpecialRoomType.BossEntrance) {
				return new Texture2D(10, 10);
			}
		}
		return null;
	}

	public override void ModifyAbilityData(AbilityType type, AbilityData data) {
		if (type != AbilityType.ShoutTalent) {
			return;
		}

		data.CooldownDecreasePerHit = true;
		data.CooldownTime = 20;
	}
}