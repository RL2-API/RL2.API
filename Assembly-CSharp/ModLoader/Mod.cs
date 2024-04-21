using System;
using System.Reflection;
using UnityEngine;

namespace RL2.ModLoader;

public abstract class Mod
{
	/// <summary>
	/// Declares what version the mod is in.
	/// </summary>
	public abstract int[] Version { get; }
	/// <summary>
	/// All ModSystems added by this mod. Filled during mod loading.
	/// </summary>
	internal Type[] ModSystems;
	/// <summary>
	/// All ModPlayers added by this mod. Filled during mod loading.
	/// </summary>
	internal Type[] ModPlayers;
	/// <summary>
	/// Ran right after loading all mods.
	/// </summary>
	public virtual void OnLoad() { }
	/// <summary>
	/// Prints the specified message to logs and console.
	/// </summary>
	/// <param name="message">Text to be printed</param>
	public static void Log(string message) {
		string callingAssemblyName = Assembly.GetCallingAssembly().GetName().Name;
		Debug.Log($"[{callingAssemblyName}]: {message}");
	}
}
