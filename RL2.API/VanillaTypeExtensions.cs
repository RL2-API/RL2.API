using System.Collections.Generic;
using UnityEngine;

namespace RL2.ModLoader;

/// <summary>
/// Extension methods for vanilla RL2 types
/// </summary>
public static class	VanillaTypeExtensions
{
	/// <summary>
	/// Provides access to the stat bonuses
	/// </summary>
	/// <param name="playerController"></param>
	/// <returns></returns>
	public static PlayerStatBonuses StatBonuses(this PlayerController playerController) => RL2API.StatBonuses;

	/// <summary>
	/// Provides access to the dictionary containing map icons
	/// </summary>
	/// <param name="mapController"></param>
	/// <returns></returns>
	public static Dictionary<string, GameObject> TextureHashToPrefab(this MapController mapController) => RL2API.TextureHashToPrefab;
}