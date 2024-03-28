using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RL2.ModLoader;

public class ModLoader
{

	public static readonly string dataPath = Application.dataPath.Replace("/", "\\");
	public static readonly string ModPath = dataPath + "\\Mods"; // No, it cannot be const
	public static Mod[] LoadedMods;
	public static List<Type> LoadedModPlayers = new();

	public static void LoadMods()
	{
		DirectoryInfo directory = new DirectoryInfo(ModPath);
		FileInfo[] files = directory.GetFiles("*.dll", SearchOption.TopDirectoryOnly);

		foreach (FileInfo file in files)
		{
			AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(file.FullName));
			Log("Found " + file.FullName.Replace(dataPath, ""));
		}

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

			CommandManager.RegisterCommands(assembly);
			LoadedMods[modCount] = mod;
			modCount++;
		}
	}

	public static void Log(string message)
	{
		Debug.Log($"[ModLoader]: {message}");
	}
}
