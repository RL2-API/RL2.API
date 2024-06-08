using Rewired.Utils.Libraries.TinyJson;
using System;
using System.Linq;

namespace RL2.ModLoader;

/// <summary>
/// Decalres the base info about the mod. A JSON file containing this information has a "*.mod.json" extension
/// </summary>
[Serializable]
public class ModManifest
{
	/// <summary>
	/// Mod name
	/// </summary>
	public string Name;
	/// <summary>
	/// Mod author
	/// </summary>
	public string Author;
	/// <summary>
	/// Mod version. Must follow the <see href="https://semver.org/">symantic versioning scheme</see>
	/// </summary>
	[Serialize]
	internal string ModVersion;
	/// <summary>
	/// Relative path from the mod manifest to the mods assembly
	/// </summary>
	public string AssemblyPath;
	/// <summary>
	/// List of all mod names which take priority in loading
	/// </summary>
	public string[] LoadAfter;
	/// <summary>
	/// Version of the mod as a semantic version object
	/// </summary>
	[DoNotSerialize]
	public SemVersion Version => new SemVersion(ModVersion);

	public int CompareTo(ModManifest comparedObject) {
		if (comparedObject == null) {
			return -1;
		}

		if (Name == comparedObject.Name) {
			return Version.CompareTo(comparedObject.Version);
		}

		return LoadAfter.Contains(comparedObject.Name) ? 1 : 0;
	}
}