using System.IO;
using System.Reflection;
using System;
using UnityEngine;
using System.Linq;

namespace RL2.ModLoader;

public class ModLoader {
    public static readonly string ModPath = Application.dataPath.Replace("/", "\\") + "\\Mods"; // No, it cannot be const, and why in the world does dataPath use "/"?

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
        // This is mainly for debugging and the only useful thing here...
        foreach (Assembly assembly in modAssemblies)
        {
            Log(assembly.GetName().Name);
            Log(assembly.GetName().FullName);
            Log(assembly.Location);

            // ... is invoking the OnLoad method
            Type[] types = assembly.GetTypes().Where(x => x.BaseType.FullName == "RL2.ModLoader.Mod").ToArray();
            foreach (Type type in types)
            {
                var mod = assembly.CreateInstance(type.FullName);
                type.GetMethod("OnLoad").Invoke(mod, null);
            }
        }
        Log(ModPath);
    }

    public static void Log(string message)
    {
        Debug.Log($"[ModLoader]: {message}");
    }
}