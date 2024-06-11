using RL2.ModLoader.APIEndpoints;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RL2.ModLoader;

/// <summary>
/// Main entrypoint for the RL2.API
/// </summary>
public class APIStore
{
	/// <summary>
	/// All loaded mods using RL2.API
	/// </summary>
	public static Mod[] LoadedMods = [];

	/// <summary>
	/// A list of all names of loaded mods
	/// </summary>
	public static List<string> LoadedModNames = [];

	/// <summary>
	/// 
	/// </summary>
	public static APIEndpointManager APIEndpoints = new APIEndpointManager();

	/// <summary>
	/// 
	/// </summary>
	public APIStore() {
		Mod.Log("RL2.API loaded");
		LoadedModNames = [];
		LoadedMods = LoadAPICompliantMods();
		Mod.Log($"Disabled mods: {string.Join(" | ", ModLoader.ModList?.Disabled)}");
		Mod.Log($"Enabled mods: {string.Join(" | ", LoadedModNames)}");
	}

	/// <summary></summary>
	/// <returns>An array of all mods that succeded to load</returns>
	public Mod[] LoadAPICompliantMods() {
		List<Mod> loadedMods = [];
		ModManifest[] notDisabledModManifests = ModLoader.ModManifests.Where(manifest => ModLoader.ModList?.Disabled.IndexOf(manifest.Name) == -1).ToArray();
		Assembly?[] modAssemblies = GetEnabledModAssemblies(notDisabledModManifests);
		for (int i = 0; i < modAssemblies.Length; i++) {
			Mod? mod = TryLoadMod(modAssemblies[i], notDisabledModManifests[i]);
			if (mod != null) {
				loadedMods.Add(mod);
				LoadedModNames.Add($"{notDisabledModManifests[i].Name} v{notDisabledModManifests[i].Version}");
				continue;
			}
		}
		return loadedMods.ToArray();
	}

	/// <summary>
	/// While examining the mods manifest we check whether:<br/>
	/// - A mod with this name and a bigger version was already loaded;<br/>
	/// - Whether this mod is on the list of enabled mods - if not, we add it to the list;<br/>
	/// - Whether this mods assmebly exists;<br/>
	/// - Whether this mods assembly classifies it as an RL2.API compliant mod;
	/// </summary>
	/// <returns>An array of all main assemblies of enabled mods</returns>
	public Assembly?[] GetEnabledModAssemblies(ModManifest[] modManifests) {
		Assembly?[] modAssemblies = new Assembly?[modManifests.Length];
		string[] modNames = [];
		int currentModID = -1;
		foreach (ModManifest modManifest in modManifests) {
			currentModID++;
			string modName = modManifest.Name;

			if (modNames.IndexOf(modName) != -1) {
				Mod.Log($"A newer version of {modName} is already loaded");
				modAssemblies[currentModID] = null;
				continue;
			}

			if (ModLoader.ModList?.Enabled.IndexOf(modName) == -1) {
				Mod.Log($"New mod \"{modName}\" was found, and it was automatically enabled");
				ModLoader.ModList.Enabled.Add(modName);
			}

			string modAssemblyPath = ModLoader.ModManifestPaths?[modManifest] + "\\" + modManifest.ModAssembly;
			if (!File.Exists(modAssemblyPath)) {
				Mod.Log($"Assembly with path {modAssemblyPath} was not found");
				modAssemblies[currentModID] = null;
				continue;
			}

			var modAssembly = Assembly.LoadFrom(modAssemblyPath);
			if (modAssembly.GetTypes().Where(type => type.IsSubclassOf(typeof(Mod))).Count() != 1) {
				Mod.Log($"{modName} is not a valid mod assembly, as it should only contain a single Mod class");
				modAssemblies[currentModID] = null;
				continue;
			}

			modAssemblies[currentModID] = AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(modAssemblyPath));
			modNames.Add(modName);
		}

		return modAssemblies;
	}

	/// <summary>
	/// Creates an instance of <see cref="Mod"/> from the assembly
	/// </summary>
	/// <param name="assembly">A mods assembly</param>
	/// <param name="manifest">A mods manifest</param>
	/// <returns>
	/// <see cref="Mod"/> if creation succeded;<br></br>
	/// <see langword="null"/> if the provided assembly is <see langword="null"/> or creation failed;
	/// </returns>
	public Mod? TryLoadMod(Assembly? assembly, ModManifest manifest) {
		if (assembly == null) {
			return null;
		}

		Type modType = assembly.GetTypes().First(type => type.IsSubclassOf(typeof(Mod)));
		Mod? mod = null;
		try {
			mod = (Mod)Activator.CreateInstance(modType);
		}
		catch (Exception ex) {
			Mod.Log($"Failed to load {manifest.Name}: {ex}");
		}
		if (mod == null) {
			return null;
		}

		mod.Path = ModLoader.ModManifestPaths?[manifest] + "\\" ?? ModLoader.ModPath;
		mod.Content = assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(ModType))).ToArray();
		return mod;
	}

	/// <summary>
	/// Retrieves the instance of the provided mod
	/// </summary>
	/// <typeparam name="T">Wanted mod</typeparam>
	/// <returns>
	/// Stored instance of found mod;
	/// </returns>
	public static T GetModInstance<T>() where T : Mod {
		foreach (Mod mod in LoadedMods) {
			if (mod is T) {
				return (T)mod;
			}
		}
		return null;
	}
}