using System.IO;
using UnityEngine;

namespace RL2.ModLoader;

public class BuiltinCommands
{
	public static readonly string modSources = ModLoader.dataPath + "\\ModSources";

	public static void GenerateModSkeleton(string[] args) {
		string modName = "";
		if (args.Length != 1) {
			ModLoader.Log("\"generate-mod-skeleton\" command takes exactly one argument: mod name");
			return;
		}
		modName = args[0];

		string[] csprojContents = new string[] {
			"<Project Sdk=\"Microsoft.NET.Sdk\">",
			"",
			"	<PropertyGroup>",
			"		<TargetFramework>net48</TargetFramework>",
			"		<GenerateAssemblyInfo>False</GenerateAssemblyInfo>",
			"		<LangVersion>10</LangVersion>",
			"	</PropertyGroup>",
			"",
			"	<ItemGroup>",
			$"		<Reference Include=\"..\\..\\Managed\\*.dll\" Exclude=\"..\\..\\Managed\\Assembly-CSharp-original.dll\">",
			"			<Private>false</Private>",
			"		</Reference>",
			"	</ItemGroup>",
			"",
			"</Project>"
		};

		if (!Directory.Exists(modSources)) {
			Directory.CreateDirectory(modSources);
		}
		string newModPath = modSources + $"\\{modName}";
		if (Directory.Exists(newModPath)) {
			ModLoader.Log($"A mod with this name: {modName} already exists in your ModSources directory");
			return;
		}
		Directory.CreateDirectory(newModPath);
		File.WriteAllLines(newModPath + $"\\{modName}.csproj", csprojContents, System.Text.Encoding.UTF8);
		ModManifest modManifest = new ModManifest();
		modManifest.Name = modName;
		modManifest.Version = "0.0.0";
		modManifest.ModAssembly = $"bin\\Debug\\net40\\{modName}.dll";
		File.WriteAllText(newModPath + $"\\{modName}.mod.json", JsonUtility.ToJson(modManifest, true));

		string[] modFileContent = new string[] {
			"using RL2.ModLoader;",
			"",
			$"namespace {modName};",
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