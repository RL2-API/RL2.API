using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RL2.ModLoader;

public class ModLoader
{
	public static readonly string Version = "-experimental";
	public static readonly string dataPath = Application.dataPath.Replace("/", "\\");
	public static readonly string ModPath = dataPath + "\\Mods";
	public static Mod[] LoadedMods;

	class ModLoadData
	{
		public List<string> enabled;
		public List<string> disabled;
	}

	public static void LoadMods() {
		// Create the Mods directory if it doesn't exist
		if (!Directory.Exists(ModPath)) {
			Directory.CreateDirectory(ModPath);
		}
		// Create the enabled.json file if it doesn't exist
		if (!File.Exists(ModPath + "\\enabled.json")) {
			File.WriteAllText(ModPath + "\\enabled.json", JsonUtility.ToJson(new ModLoadData()));
		}

		ModLoadData modLoadData = JsonUtility.FromJson<ModLoadData>(File.ReadAllText(ModPath + "\\enabled.json"));

		// Get all manifest files from ModPath
		DirectoryInfo directory = new DirectoryInfo(ModPath);
		FileInfo[] fileInfos = directory.GetFiles("*.mod.json", SearchOption.AllDirectories);
		
		// Create ModManifest insatnces from data in the found manifests
		ModManifest[] modManifests = new ModManifest[fileInfos.Length];
		for (int i = 0; i < fileInfos.Length; i++) {
			modManifests[i] = JsonUtility.FromJson<ModManifest>(File.ReadAllText(fileInfos[i].FullName));
		}
		// Sort ModManifest instances, see ModManifest.ComapreTo for more info
		Array.Sort(modManifests);

		// Load needed assemblies
		List<string> loadedManifests = new List<string>();
		foreach (ModManifest modManifest in modManifests) {
			if (modLoadData.disabled.Contains(modManifest.Name)) {
				Log($"<color=red>{modManifest.Name} is on the disabled list. You can change it in enabled.json</color=red>");
				continue;
			}

			if (!modLoadData.enabled.Contains(modManifest.Name)) {
				Log($"<color=green>New mod added: {modManifest.Name}</color=green>");
				modLoadData.enabled.Add(modManifest.Name);
			}

			if (loadedManifests.Contains(modManifest.Name)) {
				Log($"A newer version of {modManifest.Name} was alredy loaded");
				continue;
			}

			AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName($"{ModPath}\\{modManifest.Name}\\{modManifest.ModAssembly}"));
			foreach (string dependency in modManifest.AdditionalDependencies) {
				AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName($"{ModPath}\\{modManifest.Name}\\{dependency}"));
			}
			Log($"{modManifest.Name} was loaded");
		}

		File.WriteAllText(ModPath + "\\enabled.json", JsonUtility.ToJson(modLoadData));

		//Get all assemblies for ModPath containing at least 1 Mod class
		Assembly[] modAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(
			assembly => 
				assembly.Location.StartsWith(ModPath) && 
				assembly.DefinedTypes.Where(type => type.IsSubclassOf(typeof(Mod))).Count() >= 1
		).ToArray();

		LoadedMods = new Mod[modAssemblies.Length];
		int currentMod = 0;
		foreach (Assembly assembly in modAssemblies) {
			Type[] modTypes = assembly.GetTypes().Where(x => x.BaseType.FullName == typeof(Mod).FullName).ToArray();
			if (modTypes.Length != 1) {
				Log($"<color=red>Make sure that the mod contains <b>exactly one Mod class</b>, {assembly.GetName().Name} will not be loaded as this condition was not met</color>");
				continue;
			}
			Mod mod = (Mod)Activator.CreateInstance(modTypes[0]);
			mod.Content = assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(ModType))).ToArray();

			CommandManager.RegisterCommands(assembly);
			LoadedMods[currentMod] = mod;
			currentMod++;
		}
		foreach (Mod mod in LoadedMods) {
			mod.OnLoad();
		}
	}

	public static void Log(object message) {
		Debug.Log($"[ModLoader]: {message}");
	}
}
