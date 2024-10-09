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
	public string Path { get; internal set; }

	/// <summary>
	/// All registrable types from this mod
	/// </summary>
	internal Type[] RegistrableContent;

	/// <summary>
	/// Ran right after loading all mods.
	/// </summary>
	public virtual void OnLoad() { }
	
	/// <summary>
	/// 
	/// </summary>
	public virtual void OnUnload() { }

	internal void SetupContent() {
		foreach (Type type in Content) { 
			var instance = Activator.CreateInstance(type);
			if (instance is IRegistrable registrable) {
				registrable.Register();
			}
		}
	}
	
	/// <summary>
	/// Logs the message with your "[YourModClassName]"
	/// </summary>
	/// <param name="message"></param>
	public static void Log(object message) {
		string name = Assembly.GetCallingAssembly().GetName().Name == "RL2.API" ? "RL2.API" : Assembly.GetCallingAssembly().GetTypes().First(type => type.IsSubclassOf(typeof(Mod))).Name;
		Debug.Log($"[{name}]: {message}");
	}
}