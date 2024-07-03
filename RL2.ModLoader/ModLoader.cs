using Rewired.Utils.Libraries.TinyJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RL2.ModLoader;

/// <summary>
/// Main entrypoint of the mod loader
/// </summary>
public partial class ModLoader
{
	/// <summary>
	/// Path to directory containing all mods
	/// </summary>
	public static readonly string ModPath = Application.dataPath.Replace("/", "\\") + "\\Mods";

	/// <summary>
	/// Path to the file containing the enabled/disabled mods list
	/// </summary>
	public static readonly string ModListPath = ModPath + "\\enabled.json";

	/// <summary>
	/// Stores lists of enabled/disabled mods
	/// </summary>
	public static ModList ModList = new ModList();

	/// <summary>
	/// Stores all mod manifests and their paths
	/// </summary>
	public static SortedDictionary<ModManifest, string> ModManifestToPath = [];

	/// <summary>
	/// Stores names of all loaded mods
	/// </summary>
	public static Dictionary<string, SemVersion> LoadedModNamesToVersions = [];

	/// <summary>
	/// Stores all independent mods
	/// </summary>
	public static List<object> IndependentModObjects = [];

	/// <summary>
	/// Initializes the modloader
	/// </summary>
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	public static void Initialize() {
		EnsureModsDirectorysExists();
		CreateModList();
		LoadModManifests();
		FilterModManifests();
		LoadMods();
		LogLoaded();
	}

	/// <summary>
	/// Ensures that the "Mods" directory exists
	/// </summary>
	public static void EnsureModsDirectorysExists() {
		if (!Directory.Exists(ModPath)) {
			Directory.CreateDirectory(ModPath);
		}
	}

	/// <summary>
	/// Loads the <see cref="ModList"/> object
	/// </summary>
	public static void CreateModList() {
		if (!File.Exists(ModListPath)) {
			File.WriteAllText(ModListPath, "");
		}
		if (JsonParser.FromJson<ModList>(File.ReadAllText(ModListPath)) == null) {
			File.WriteAllText(ModListPath, JsonWriter.ToJson(new ModList()).Prettify());
		}
		ModList = JsonParser.FromJson<ModList>(File.ReadAllText(ModListPath));
	}

	/// <summary>
	/// Fills the ModManifests list as well as the ModManifestPaths dictionary with found entries
	/// </summary>
	public static void LoadModManifests() {
		// Get all manifest files from ModPath
		DirectoryInfo directory = new DirectoryInfo(ModPath);
		FileInfo[] fileInfos = directory.GetFiles("*.mod.json", SearchOption.AllDirectories);

		for (int i = 0; i < fileInfos.Length; i++) {
			ModManifest currentManifest = JsonParser.FromJson<ModManifest>(File.ReadAllText(fileInfos[i].FullName));
			ModManifestToPath.Add(currentManifest, fileInfos[i].Directory.FullName);
		}
	}

	/// <summary>
	/// Filters out the disabled mods
	/// </summary>
	public static void FilterModManifests() {
		ModManifest[] notDiasbledManifests = ModManifestToPath.Keys.Where(entry => !ModList.Disabled.Contains(entry.Name)).ToArray();
		SortedDictionary<ModManifest, string> temporaryDictionary = [];
		foreach (ModManifest manifest in notDiasbledManifests) {
			temporaryDictionary.Add(manifest, ModManifestToPath[manifest]);
		}
		ModManifestToPath = temporaryDictionary;
	}

	/// <summary>
	/// Loads mods
	/// </summary>
	public static void LoadMods() {
		foreach (ModManifest manifest in ModManifestToPath.Keys) {
			if (LoadedModNamesToVersions.Keys.Contains(manifest.Name)) {
				continue;
			}

			if (!ModList.Enabled.Contains(manifest.Name)) {
				ModList.Enabled.Add(manifest.Name);
			}

			Assembly modAssembly = Assembly.LoadFrom(ModManifestToPath[manifest] + "\\" + manifest.ModAssembly);
			Type[] mods = modAssembly.GetTypes().Where(t => t.GetCustomAttribute<ModEntrypointAttribute>() != null).ToArray();
			foreach (Type mod in mods) {
				LoadedModNamesToVersions.Add(manifest.Name, manifest.SemVersion);
				IndependentModObjects.Add(Activator.CreateInstance(mod));
			}
		}
	}

	/// <summary>
	/// Logs all loaded mods
	/// </summary>
	public static void LogLoaded() {
		List<string> loaded = [];
		foreach (KeyValuePair<string, SemVersion> entry in LoadedModNamesToVersions) {
			loaded.Add($"{entry.Key} v{entry.Value}");
		}
		ModLoader.Log("Loaded mods: " + string.Join(" | ", loaded));
		ModLoader.Log("Disabled mods: " + string.Join(" | ", ModList.Disabled));
	}

	/// <summary>
	/// Logs the message with "[RL2-Modloader]: " prepended
	/// </summary>	
	/// <param name="message">The message to log</param>
	public static void Log(object message) {
		Debug.Log($"[RL2-ModLoader]: " + message);
	}
}