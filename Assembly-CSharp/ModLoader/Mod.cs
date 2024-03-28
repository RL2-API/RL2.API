using System.Reflection;

namespace RL2.ModLoader;

public abstract class Mod
{
	public virtual void Load() { }

	public static void Log(string message)
	{
		string callingAssemblyName = Assembly.GetCallingAssembly().GetName().Name;
		Debug.Log($"[{callingAssemblyName}]: {message}");
	}
}
