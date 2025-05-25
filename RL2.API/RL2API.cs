using RL2.ModLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
	/// 
	/// </summary>
	public RL2API() {
		Log("RL2.API loaded");
		LoadedMods = LoadAPICompliantMods();
	}

	/// <summary></summary>
	/// <returns>An array of all mods that succeded to load</returns>
	public Mod[] LoadAPICompliantMods() {
		List<Mod> loadedMods = [];
		ModManifest[] notDisabledModManifests = ModLoader.ModLoader.ModManifestToPath.Keys.Where(
			manifest =>
				ModLoader.ModLoader.ModList?.Disabled.IndexOf(manifest.Name) == -1 &&
				manifest.LoadAfter.Contains("RL2.API")
		).ToArray();
		Assembly?[] modAssemblies = GetEnabledModAssemblies(notDisabledModManifests);
		for (int i = 0; i < modAssemblies.Length; i++) {
			Mod? mod = TryLoadMod(modAssemblies[i], notDisabledModManifests[i]);
			if (mod != null) {
				loadedMods.Add(mod);
				ModLoader.ModLoader.LoadedModNamesToVersions.Add(notDisabledModManifests[i].Name, notDisabledModManifests[i].SemVersion);
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
				Log($"A newer version of {modName} is already loaded");
				modAssemblies[currentModID] = null;
				continue;
			}

			if (ModLoader.ModLoader.ModList?.Enabled.IndexOf(modName) == -1) {
				Log($"New mod \"{modName}\" was found, and it was automatically enabled");
				ModLoader.ModLoader.ModList.Enabled.Add(modName);
			}

			string modAssemblyPath = ModLoader.ModLoader.ModManifestToPath?[modManifest] + "\\" + modManifest.ModAssembly;
			if (!File.Exists(modAssemblyPath)) {
				Log($"Assembly with path {modAssemblyPath} was not found");
				modAssemblies[currentModID] = null;
				continue;
			}

			var modAssembly = Assembly.LoadFrom(modAssemblyPath);
			if (modAssembly.GetTypes().Where(type => type.GetCustomAttribute<ModEntrypointAttribute>() != null).ToArray().Length != 0) {
				modAssemblies[currentModID] = null;
				continue;
			}
			if (modAssembly.GetTypes().Count(type => type.IsSubclassOf(typeof(Mod))) != 1) {
				Log($"{modName} is not a valid mod assembly, as it should only contain a single Mod class");
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
			Log($"Failed to load {manifest.Name}: {ex}");
		}
		if (mod == null) {
			return null;
		}

		mod.Path = ModLoader.ModLoader.ModManifestToPath?[manifest] + "\\" ?? ModLoader.ModLoader.ModPath;
		mod.Manifest = manifest;
		mod.RegistrableContent ??= assembly.GetTypes().Where(type => typeof(IRegistrable).IsAssignableFrom(type)).ToArray();
		CommandManager.RegisterCommands(assembly);
		ModLoader.ModLoader.OnLoad += mod.OnLoad;
		ModLoader.ModLoader.OnUnload += mod.OnUnload;
		return mod;
	}

	/// <summary>
	/// Retrieves the instance of the provided mod
	/// </summary>
	/// <typeparam name="T">Wanted mod</typeparam>
	/// <returns>
	/// Stored instance of found mod;
	/// <see langword="null"/> if not found
	/// </returns>
	public static T? GetModInstance<T>() where T : Mod {
		foreach (Mod mod in LoadedMods) {
			if (mod is T) {
				return (T)mod;
			}
		}
		return null;
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

	/// <summary>
	/// 
	/// </summary>
	/// <param name="modType"></param>
	/// <returns></returns>
	public static Mod? GetModInstance(Type modType) {
		foreach (Mod mod in LoadedMods) {
			if (mod.GetType() == modType) {
				return mod;
			}
		}

		return null;
	}

	internal static void Log(object message) {
		UnityEngine.Debug.Log($"[RL2.API]: {message}");
	}
}