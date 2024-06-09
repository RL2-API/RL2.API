using Rewired.Utils.Libraries.TinyJson;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RL2.ModLoader;

/// <summary>
/// Class responsible for loading the RL2.API and all mods
/// </summary>
public partial class ModLoader
{
	/// <summary>
	/// Expected path to the RL2.API
	/// </summary>
	public static readonly string APIPath = Application.dataPath.Replace("/", "\\") + "\\Managed\\RL2.API.dll";

	/// <summary>
	/// Path to directory containing all mods
	/// </summary>
	public static readonly string ModPath = Application.dataPath.Replace("/", "\\") + "\\Mods";

	/// <summary>
	/// Path to the file containing the enabled/disabled mods list
	/// </summary>
	public static readonly string ModListPath = ModPath + "\\enabled.json";

	/// <summary>
	/// A <see cref="ModList"/> object representing all enabled and disabled mods
	/// </summary>
	public static ModList? ModList;

	/// <summary>
	/// All found ModManifests
	/// </summary>
	public static ModManifest[] ModManifests = [];

	/// <summary>
	/// Key: one of the found ModManifests;<br/>
	/// Value: path to the directory in which the ModManifest is located
	/// </summary>
	public static Dictionary<ModManifest, string> ModManifestPaths = [];

	/// <summary>
	/// Stores a refernece to the RL2.API's APIStore.Is <see langword="null"/> if the RL2.API hasn't been loaded
	/// </summary>
	public static object? APIStore;

	/// <summary>
	/// Initializes the modloader
	/// </summary>
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	public static void Initialize() {
		EnsureBasicDirectoriesExist();
		ModList = JsonParser.FromJson<ModList>(File.ReadAllText(ModListPath));
		GetModManifests();
		APIStore = LoadAPI();
		LoadNonAPICompliantMods();
	}

	/// <summary>
	/// Ensures that all directories and files necessary for the modlaoders functionality are present
	/// </summary>
	public static void EnsureBasicDirectoriesExist() {
		if (!Directory.Exists(ModPath)) {
			Directory.CreateDirectory(ModPath);
		}
		if (!File.Exists(ModListPath)) {
			File.WriteAllText(ModListPath, JsonWriter.ToJson(new ModList() {
				Enabled = [],
				Disabled = []
			}).Prettify());
		}
	}

	/// <summary>
	/// Fills the ModManifests list as well as the ModManifestPaths dictionary with found entries
	/// </summary>
	public static void GetModManifests() {
		// Get all manifest files from ModPath
		DirectoryInfo directory = new DirectoryInfo(ModPath);
		FileInfo[] fileInfos = directory.GetFiles("*.mod.json", SearchOption.AllDirectories);
		ModManifestPaths = new Dictionary<ModManifest, string>();

		// Create ModManifest insatnces from data in the found manifests, and save the manifests path
		ModManifests = new ModManifest[fileInfos.Length];
		for (int i = 0; i < fileInfos.Length; i++) {
			ModManifests[i] = JsonUtility.FromJson<ModManifest>(File.ReadAllText(fileInfos[i].FullName));
			ModManifestPaths.TryAdd(ModManifests[i], fileInfos[i].Directory.FullName);
		}

		Array.Sort(ModManifests);
	}

	/// <summary>
	/// Loads the RL2.API
	/// </summary>
	/// <returns>
	/// An object representing the main entrypoint of the RL2.AP;<br/>
	/// <see langword="null"/> if the RL2.API was not loaded;
	/// </returns>
	public static object? LoadAPI() {
		if (File.Exists(APIPath)) {
			Assembly RL2API = AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(APIPath));
			Log("Start loading RL2.API...");
			return Activator.CreateInstance(RL2API.GetTypes().Where(T => T.Name == "APIStore").ElementAt(0));
		}
		Log("RL2.API.dll not found");
		return null;
	}

	/// <summary>
	/// Loads all mods that are not compliant with the RL2.API;
	/// </summary>
	public static void LoadNonAPICompliantMods() { }

	/// <summary>
	/// Logs the message with "[RL2-Modloader]: " prepended
	/// </summary>	
	/// <param name="message">The message to log</param>
	public static void Log(object message) {
		Debug.Log($"[RL2-ModLoader]: " + message);
	}
}