using System.IO;
using System.Reflection;
using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace RL2.ModLoader;

public class ModLoader {
    public static readonly string ModPath = Application.dataPath.Replace("/", "\\") + "\\Mods"; // No, it cannot be const
    public static ModInstance[] LoadedMods;

    public static void LoadMods()
    {
        DirectoryInfo directory = new DirectoryInfo(ModPath);
        FileInfo[] files = directory.GetFiles("*.dll", SearchOption.TopDirectoryOnly);
        
        foreach (FileInfo file in files)
        {
            AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(file.FullName));
            Log($"Found {file.FullName}");
        }

        Assembly[] modAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.Location == $"{ModPath}\\{x.GetName().Name}.dll").ToArray();
        LoadedMods = new ModInstance[modAssemblies.Length];
        int modCount = 0;
        foreach (Assembly assembly in modAssemblies)
        {
            Type[] types = assembly.GetTypes();
            Mod modClassInstance = new Mod();
            List<ModSystem> modSystems = new();
            foreach (Type type in types)
            {
                switch (type.BaseType.FullName) {
                    case "RL2.Modloader.Mod":
                        if (type.Name != assembly.GetName().Name)
                        {
                            Log($"Failed to load the {type.Name} Mod class - the Mod class should be named the same as your assembly");
                            break;
                        }
                        modClassInstance = (Mod)Activator.CreateInstance(type);
                        break;
                    case "RL2.ModLoader.ModSystem":
                        modSystems.Add((ModSystem)Activator.CreateInstance(type));
                        break;
                }
            }

            ModInstance mod = new ModInstance(modClassInstance, modSystems.ToArray());
            LoadedMods[modCount] = mod;
            modCount++;
        }
        Log(ModPath);
    }

    public static void Log(string message)
    {
        Debug.Log($"[ModLoader]: {message}");
    }
}