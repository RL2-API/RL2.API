using Rewired.Utils.Libraries.TinyJson;
using System;
using System.Linq;

namespace RL2.ModLoader;

/// <summary>
/// Decalres the base info about the mod. A JSON file containing this information has a "*.mod.json" extension
/// </summary>
[Serializable]
public class ModManifest : IComparable<ModManifest>
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
	public string Version;
	/// <summary>
	/// Relative path from the mod manifest to the mods assembly
	/// </summary>
	public string ModAssembly;
	/// <summary>
	/// List of all mod names which take priority in loading
	/// </summary>
	public string[] LoadAfter;
	/// <summary>
	/// Version of the mod as a semantic version object
	/// </summary>
	[DoNotSerialize]
	public SemVersion SemVersion => new SemVersion(this.Version);

	/// <summary>
	/// Compares to ModManifest objects
	/// </summary>
	/// <param name="comparedObject"></param>
	/// <returns></returns>
	public int CompareTo(ModManifest comparedObject) {
		if (this == null) {
			return comparedObject == null ? 0 : 1;
		}
		if (comparedObject == null) {
			return -1;
		}

		if (Name == comparedObject.Name) {
			return SemVersion.CompareTo(comparedObject.SemVersion);
		}

		return LoadAfter.Contains(comparedObject.Name) ? 1 : 0;
	}
}