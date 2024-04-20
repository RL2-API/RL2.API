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

		}
		/*
		Assembly[] modAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.Location == $"{ModPath}\\{x.GetName().Name}.dll").ToArray();
		LoadedMods = new Mod[modAssemblies.Length];
		int modCount = 0;
		foreach (Assembly assembly in modAssemblies)
		{
			Type[] types = assembly.GetTypes().Where(x => x.Name == assembly.GetName().Name && x.BaseType.FullName == "RL2.ModLoader.Mod").ToArray();
			Mod mod = (Mod)Activator.CreateInstance(types[0]);

			if (mod == null)
			{
				Log($"Failed to load the Mod class - the Mod class was not found");
				break;
			}

			Type[] modPlayers = assembly.GetTypes().Where(x => x.BaseType.FullName == "RL2.ModLoader.ModPlayer").ToArray();
			foreach (Type modPlayer in modPlayers)
			{
				LoadedModPlayers.Add(modPlayer);
			}

			Type[] globalEnemies = assembly.GetTypes().Where(x => x.BaseType.FullName == "RL2.ModLoader.GlobalEnemy").ToArray();
			foreach (Type globalEnemy in globalEnemies)
			{
				LoadedGlobalEnemies.Add(globalEnemy);
			}

			CommandManager.RegisterCommands(assembly);
			LoadedMods[modCount] = mod;
			modCount++;
		}
	*/
	}

	public static void Log(string message) {
		Debug.Log($"[ModLoader]: {message}");
	}
}
