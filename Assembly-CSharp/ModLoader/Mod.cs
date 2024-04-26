using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RL2.ModLoader;

/// <summary>
/// Required for every mod to be loaded.
/// </summary>
public abstract class Mod
{
	/// <summary>
	/// Declares what version the mod is in.
	/// </summary>
	public abstract string Version { get; }
	
	/// <summary>
	/// All types from this mod.
	/// </summary>
	internal Type[] Content;
	
	/// <summary>
	/// Gets all types inheriting from T.
	/// </summary>
	/// <typeparam name="T">The type you want to get derived classes of.</typeparam>
	internal Type[] GetTypes<T>() where T : ModType => Content.Where(type => type.IsSubclassOf(typeof(T))).ToArray();
	
	/// <summary>
	/// Ran right after loading all mods.
	/// </summary>
	public virtual void OnLoad() { }
	
	/// <summary>
	/// Prints the specified message to logs and console.
	/// </summary>
	/// <param name="message">Text to be printed</param>
	public static void Log(object message) {
		string callingModName = Assembly.GetCallingAssembly().GetTypes().Where(type => type.BaseType == typeof(Mod)).First().Name;
		Debug.Log($"[{callingModName}]: {message}");
	}
}
