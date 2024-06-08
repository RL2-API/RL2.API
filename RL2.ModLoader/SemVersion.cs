using System;
using System.Collections.Generic;

namespace RL2.ModLoader;

/// <summary>
/// A class representing a semantic versioning string
/// </summary>
public class SemVersion : IComparable<SemVersion>
{
	/// <summary>
	/// Major version
	/// </summary>
	public int Major;
	/// <summary>
	/// Minor version
	/// </summary>
	public int Minor;
	/// <summary>
	/// Patch version
	/// </summary>
	public int Patch;
	/// <summary>
	/// Release type
	/// </summary>
	public string? ReleaseType;
	/// <summary>
	/// Build version
	/// </summary>
	public string? BuildVersion;

	/// <summary>
	/// Creates a new SemVersion object
	/// </summary>
	/// <param name="major">Major version</param>
	/// <param name="minor">Minor version</param>
	/// <param name="patch">Patch version</param>
	/// <param name="releaseType">Release type</param>
	/// <param name="buildVersion">Build version</param>
	public SemVersion(int major = 0, int minor = 0, int patch = 0, string releaseType = "", string buildVersion = "") {
		Major = major;
		Minor = minor;
		Patch = patch;
		ReleaseType = releaseType;
		BuildVersion = buildVersion;
	}

	/// <summary>
	/// Converts semantic version string into a SemVersion object <br></br>
	/// Throws if the provided <see langword="string"/> is not a valid semantic versioning string
	/// </summary>
	/// <param name="version"></param>
	/// <exception cref="ArgumentException"></exception>
	public SemVersion(string version) {
		List<char> separators = ['.'];
		if (version.Split(separators.ToArray()).Length != 3) {
			throw new ArgumentException("Not a valid semantic versioning string");
		}
		if (version.Contains("-")) {
			separators.Add('-');
		}
		if (version.Contains("+")) {
			separators.Add('+');
		}
		if (separators.Contains('-') && separators.Contains('+')) {
			if (version.IndexOf('-') > version.IndexOf('+')) {
				throw new ArgumentException("Not a valid semantic versioning string. \"-ReleaseType\" must be after \"+BuildVersion\"");
			}
		}
		string[] parts = version.Split(separators.ToArray());
		Major = int.Parse(parts[0]);
		Minor = int.Parse(parts[1]);
		Patch = int.Parse(parts[2]);
		ReleaseType = separators.Contains('-') ? parts[3] : "";
		BuildVersion = separators.Contains('+') ? (parts.Length == 4 ? parts[3] : parts[4]) : "";
	}

	/// <summary>
	///	
	/// </summary>
	/// <returns>A semantic versioning string</returns>
	public override string ToString() {
		string semVersionString = $"{Major}.{Minor}.{Patch}";
		if (ReleaseType != "") {
			semVersionString += $"-{ReleaseType}"; 
		}
		if (BuildVersion != "") {
			semVersionString += $"+{BuildVersion}";
		}
		return semVersionString;
	}

	/// <summary>
	/// Compares to SemVersion objects
	/// </summary>
	/// <param name="other"></param>
	/// <returns>-1 if this object precedes the other<br></br>0 if both objects are equal<br></br>1 if this objects succedes the other</returns>
	public int CompareTo(SemVersion? other) {
		if (other == null) {
			return -1;
		}
		if (Major > other.Major) {
			return -1;
		}
		if (Minor > other.Minor) {
			return -1;
		}
		if (Patch > other.Patch) {
			return -1;
		}
		if (ReleaseType != other.ReleaseType) {
			return ReleaseType.CompareTo(other.ReleaseType);
		}
		return BuildVersion.CompareTo(other.BuildVersion);
	}
}