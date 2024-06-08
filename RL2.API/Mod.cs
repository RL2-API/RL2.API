using System.Reflection;
using UnityEngine;

namespace RL2.ModLoader;

public abstract class Mod
{
	public virtual void OnLoad() { }
	public virtual void OnUnload() { }
	public static void Log(object message) {
		Debug.Log($"[{Assembly.GetCallingAssembly().GetName().Name}]: {message}");
	}
}