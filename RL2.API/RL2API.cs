using System;
using System.Collections.Generic;
using System.Reflection;

namespace RL2.API;

/// <summary>
/// Main entrypoint for the RL2.API
/// </summary>
[ModEntrypoint]
public partial class RL2API
{
	/// <summary>
	/// All loaded mods using RL2.API
	/// </summary>
	public static Mod[] LoadedMods = [];

	/// <summary>
	/// Contains assembly - mod instance pairs
	/// </summary>
	public static Dictionary<Assembly, Mod> AssemblyToMod = [];

	/// <summary>
	/// Initializing the API, woohoo
	/// </summary>
	public RL2API() {
		Log("Starting loading...");
	}


	/// <summary>
	/// Retrieves the instance of the provided mod
	/// </summary>
	/// <param name="modName">The name of the mod you are searching for, specified in the mods manifest</param>
	/// <returns>
	/// Stored instance of found mod;
	/// <see langword="null"/> if not found
	/// </returns>
	public static Mod? GetModInstance(string modName) {
		foreach (Mod mod in LoadedMods) {
			if (mod.Manifest.Name == modName) {
				return mod;
			}
		}

		return null;
	}

	internal static void Log(object message) {
		UnityEngine.Debug.Log($"[RL2.API]: {message}");
	}
}