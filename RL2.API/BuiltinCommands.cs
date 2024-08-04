using System.IO;
using UnityEngine;

namespace RL2.ModLoader;

/// <summary>
/// Store for all builtin RL2.API commands
/// </summary>
public class BuiltinCommands
{
	/// <summary>
	/// Creates a new mod
	/// </summary>
	/// <param name="args"></param>
	public static void NewMod(string[] args) {
		string modName = "";
		if (args.Length != 1) {
			ModLoader.Log("\"new-mod\" command takes exactly one argument: mod name");
			return;
		}
		modName = args[0];

		string[] csprojContents = new string[] {
			"<Project Sdk=\"Microsoft.NET.Sdk\">",
			"",
			"	<PropertyGroup>",
			"		<TargetFramework>net48</TargetFramework>",
			"		<GenerateAssemblyInfo>False</GenerateAssemblyInfo>",
			"		<LangVersion>latest</LangVersion>",
			"	</PropertyGroup>",
			"",
			"	<ItemGroup>",
			"		<Reference Include=\"..\\..\\Managed\\*.dll\">",
			"			<Private>false</Private>",
			"		</Reference>",
			"",
			"		<Reference Include=\"..\\RL2.API\\RL2.API.dll\">",
			"			<Private>false</Private>",
			"		</Reference>",
			"	</ItemGroup>",
			"",
			"</Project>"
		};

		string newModPath = ModLoader.ModPath + $"\\{modName}";
		if (Directory.Exists(newModPath)) {
			ModLoader.Log($"A mod with this name: {modName} already exists in your Mods directory");
			return;
		}

		Directory.CreateDirectory(newModPath);
		File.WriteAllLines(newModPath + $"\\{modName}.csproj", csprojContents, System.Text.Encoding.UTF8);
		ModManifest modManifest = new ModManifest() {
			Name = modName,
			Author = "",
			Version = "1.0.0",
			ModAssembly = $"bin/Debug/net48/{modName}.dll",
			LoadAfter = ["RL2.API"]
		};
		File.WriteAllText(newModPath + $"\\{modName}.mod.json", JsonUtility.ToJson(modManifest, true));

		string[] modFileContent = new string[] {
			"using RL2.ModLoader;",
			"",
			$"namespace {modName};",
			"",
			$"public class {modName} : Mod",
			"{",
			"	public override void OnLoad() {",
			$"		Mod.Log(\"{modName} was loaded!\");",
			"	}",
			"}"
		};
		File.WriteAllLines(newModPath + $"\\{modName}.cs", modFileContent, System.Text.Encoding.UTF8);
		ModLoader.Log($"Mod {modName} was created");
	}
}
