using RL2.ModLoader;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RL2.API;

/// <summary>
/// A clas required for your mod to be recognised as using the RL2.API
/// </summary>
public abstract class Mod
{
	/// <summary>
	/// Path to the mod, set at load
	/// </summary>
	public string Path { get; internal set; } = "";

	/// <summary>
	/// The .dll file of this mod
	/// </summary>
	public Assembly Code { get; internal set; }

	/// <summary>
	/// The manifest object of this mod
	/// </summary>
	public ModManifest Manifest { get; internal set; } = new ModManifest();

	/// <summary>
	/// All registrable types from this mod
	/// </summary>
	public Type[]? RegistrableContent;

	/// <summary>
	/// Ran right after loading all mods.
	/// </summary>
	public virtual void OnLoad() { }

	/// <summary>
	/// Ran right before unlaoding all mods
	/// </summary>
	public virtual void OnUnload() { }

	/// <summary>
	/// Registers all content instances
	/// </summary>
	public virtual void RegisterContent() {
		foreach (Type type in RegistrableContent ?? []) {
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
		string name = RL2API.AssemblyToMod[Assembly.GetCallingAssembly()].Manifest.Name;
		Debug.Log($"[{name}]: {message}");
	}
}