using UnityEngine;

namespace RL2.ModLoader;

public abstract class Mod
{
    public abstract string Name { get; }

    public virtual void Load() { }
    
    public static void Log(string message)
    {
        ModLoader.Log(message);
    }
}