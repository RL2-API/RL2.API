using System.Reflection;
using UnityEngine;

namespace RL2.ModLoader;

public abstract class Mod
{
    public abstract string Name { get; }

    public virtual void Load() { }
    
    public static void Log(string message)
	{
		string callingAssemblyName = Assembly.GetCallingAssembly().GetName().Name;
		Debug.Log($"[{callingAssemblyName}]: {message}");
	}
}