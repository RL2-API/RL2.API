using RL2.ModLoader;
using UnityEngine;

namespace ExampleMod;

internal class ExampleModSystem : ModSystem
{
	public override void ModifyGeneratedCharacter(CharacterData characterData) {
		characterData.Name = "Test";
	}

	public override Texture2D? ModifyRoomIcon(GridPointManager roomToCheck, bool getUsed, bool isMergeRoom) {
		if (roomToCheck.RoomMetaData.IsSpecialRoom) {
			if (roomToCheck.RoomMetaData.SpecialRoomType == SpecialRoomType.BossEntrance) {
				return new Texture2D(10, 10);
			}
		}
		return null;
	}
}