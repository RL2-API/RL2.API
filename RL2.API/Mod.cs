using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RL2.ModLoader;

/// <summary>
/// A clas required for your mod to be recognised as using the RL2.API
/// </summary>
public abstract class Mod
{
	/// <summary>
	/// Path to the mod, set at load
	/// </summary>
	public string Path { get; set; }

	/// <summary>
	/// All types from this mod.
	/// </summary>
	internal Type[] Content;

	/// <summary>
	/// Gets all types inheriting from T.
	/// </summary>
	/// <typeparam name="T">The type you want to get derived classes of.</typeparam>
	public Type[] GetModTypes<T>() where T : ModType => Content.Where(type => type.IsSubclassOf(typeof(T))).ToArray();

	/// <summary>
	/// Ran right after loading all mods.
	/// </summary>
	public virtual void OnLoad() { }
	
	/// <summary>
	/// 
	/// </summary>
	public virtual void OnUnload() { }
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="message"></param>
	public static void Log(object message) {
		Debug.Log($"[{Assembly.GetCallingAssembly().GetName().Name}]: {message}");
	}
}