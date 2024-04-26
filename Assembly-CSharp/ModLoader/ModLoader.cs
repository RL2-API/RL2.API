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
		// Create the Mods directory if it doesn't exist
		if (!Directory.Exists(ModPath)) {
			Directory.CreateDirectory(ModPath);
		}
		// Create the enabled.json file if it doesn't exist
		if (!File.Exists(ModPath + "\\enabled.json")) {
			File.WriteAllText(ModPath + "\\enabled.json", JsonUtility.ToJson(new ModData()));
		}

		DirectoryInfo directory = new DirectoryInfo(ModPath);
		FileInfo[] files = directory.GetFiles("*.dll", SearchOption.AllDirectories);

		foreach (FileInfo file in files) {
			AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(file.FullName));
			Log("Found " + file.FullName.Replace(ModPath, ""));
		}

		//Get all assemblies containing at least 1 Mod class
		Assembly[] modAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => assembly.Location.StartsWith(ModPath)).ToArray();
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
			mod.GlobalEnemies = assembly.GetTypes().Where(x => x.BaseType.FullName == typeof(GlobalEnemy).FullName).ToArray();

			CommandManager.RegisterCommands(assembly);
			LoadedMods[currentMod] = mod;
			currentMod++;
		}
		foreach (Mod mod in LoadedMods) {
			mod.OnLoad();
		}
	}

	public static void Log(object message) {
		if (!(message is string)) {
			message = message.ToString();
		}
		Debug.Log($"[ModLoader]: {message}");
	}
}
