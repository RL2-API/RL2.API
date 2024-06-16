using System.Collections.Generic;
using UnityEngine;

namespace RL2.ModLoader;

public static class	VanillaTypeExtensions
{
	public static PlayerStatBonuses StatBonuses(this PlayerController playerController) => RL2API.StatBonuses;

	public static Dictionary<string, GameObject> TextureHashToPrefab(this MapController mapController) => RL2API.TextureHashToPrefab;
}