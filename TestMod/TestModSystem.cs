using RL2.ModLoader;
using System.IO;
using UnityEngine;

namespace TestMod;

public class TestModSystem : ModSystem
{
	public Texture2D mapIcon = TextureExtension.LoadTexture(TestMod.Location + "\\Assets\\Naameow.png");
	public override void OnLoad() {
		Mod.Log("testmodsystemLoaded");
	}

	public override Texture2D ModifyRoomIcon(GridPointManager roomToCheck, bool getUsed, bool isMergeRoom) {
		if (roomToCheck.RoomType != RoomType.BossEntrance) {
			return null;
		}
		return mapIcon;
	}
}