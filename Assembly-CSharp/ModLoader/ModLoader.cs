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

	struct ModData
	{
		public List<string> enabled;
		public List<string> disabled;
	}

	public static void LoadMods() {
		if (!Directory.Exists(ModPath)) {
			Directory.CreateDirectory(ModPath);
		}
		if (!File.Exists(ModPath + "\\enabled.json")) {
			File.WriteAllText(ModPath + "\\enabled-exp.json", JsonUtility.ToJson(new ModData()));
		}

		DirectoryInfo directory = new DirectoryInfo(ModPath);
		FileInfo[] files = directory.GetFiles("*.dll", SearchOption.AllDirectories);

		foreach (FileInfo file in files) {
			AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(file.FullName));
			Log("Found " + file.FullName.Replace(ModPath, ""));
		}

		//Get all assemblies containing at least 1 Mod class
		Assembly[] modAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.GetTypes().Where(x => x.BaseType.FullName == typeof(Mod).FullName).ToArray().Length > 0).ToArray();
		LoadedMods = new Mod[modAssemblies.Length];
		int currentMod = 0;
		foreach (Assembly assembly in modAssemblies) {
			Type[] modTypes = assembly.GetTypes().Where(x => x.BaseType.FullName == typeof(Mod).FullName).ToArray();
			if (modTypes.Length != 1) {
				Log($"<color=red>Make sure that the mod contains <b>exactly one Mod class</b>, {assembly.GetName().Name} will not be loaded as this condition was not met</color>");
				continue;
			}
			Mod mod = (Mod)Activator.CreateInstance(modTypes[0]);
			mod.ModSystems = assembly.GetTypes().Where(x => x.BaseType.FullName == typeof(ModSystem).FullName).ToArray();
			mod.ModPlayers = assembly.GetTypes().Where(x => x.BaseType.FullName == typeof(ModPlayer).FullName).ToArray();

			CommandManager.RegisterCommands(assembly);
			LoadedMods[currentMod] = mod;
			currentMod++;
		}
	}

	public static void Log(string message) {
		Debug.Log($"[ModLoader]: {message}");
	}
}
