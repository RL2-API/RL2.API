using System.Collections.Generic;

namespace RL2.ModLoader;

/// <summary>
/// A class representing the enabled/disabled mods config
/// </summary>
public class ModList
{
	/// <summary>
	/// List of names of enabled mods
	/// </summary>
	public List<string> Enabled;
	/// <summary>
	/// List of names of disabled mods
	/// </summary>
	public List<string> Disabled;
}