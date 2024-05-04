using System;
using System.Linq;

namespace RL2.ModLoader;

public class ModManifest : IComparable<ModManifest>
{
	public string Name;
	public string Version;
	public string ModAssembly;
	public string[] AdditionalDependencies;
	public string[] LoadAfter;

	public int CompareTo(ModManifest comparedObject) {
		if (comparedObject == null) {
			return 1;
		}

		if (Name == comparedObject.Name) {
			System.Version.TryParse(Name, out var version);
			System.Version.TryParse(Name, out var version2);
			return version.CompareTo(version2);
		}

		return LoadAfter.Contains(comparedObject.Name) ? 1 : 0;
	}
}