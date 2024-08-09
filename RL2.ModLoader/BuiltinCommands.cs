using Rewired.Utils.Libraries.TinyJson;
using System.Collections.Generic;
using System.IO;

namespace RL2.ModLoader;

public partial class ModLoader
{
	/// <summary>
	/// Writes the loaded mods to the logs and console
	/// </summary>
	/// <param name="args"></param>	
	[Command("show-mods")]
	public static void ShowInstalledMods(string[] args) {
		List<string> loaded = [];
		foreach (KeyValuePair<string, SemVersion> entry in LoadedModNamesToVersions) {
			loaded.Add($"{entry.Key} v{entry.Value}");
		}
		ModLoader.Log($"Installed mods: {string.Join(" | ", loaded)}");
	}

	/// <summary>
	/// Creates base source files for a new independent mod
	/// </summary>
	/// <param name="args"></param>
	[Command("create-mod")]
	public static void CreateMod(string[] args) {
		if (args.Length == 0) {
			ModLoader.Log("No overload of \"create-mod\" takes in 0 arguments. \nCorrecct usage: \"/create-mod [ModName - required] [Author - optional]\"");
			return;
		}

		string modName = args[0];
		if (string.IsNullOrEmpty(modName)) {
			ModLoader.Log("Argument \"modName\" is required for command \"create-mod\". \nCorrecct usage: \"/create-mod [ModName - required] [Author - optional]\"");
		}

		string author = args.Length > 1 ? args[1] : "";

		string newModPath = ModLoader.ModPath + $"\\{modName}";
		if (Directory.Exists(newModPath)) {
			ModLoader.Log($"A mod with this name: {modName} already exists in your Mods directory");
			return;
		}
		Directory.CreateDirectory(newModPath);
		
		CreateCsproj(newModPath + $"\\{modName}.csproj");
		CreateModManifest(modName, author, newModPath + $"\\{modName}.mod.json");
		CreateModEntrypointFile(modName, newModPath + $"\\{modName}.cs");

		ModLoader.Log($"Mod {modName} was created");
	}

	/// <summary>
	/// Creates a new independent mod .csproj file
	/// </summary>
	/// <param name="path">Full file path with extension</param>
	public static void CreateCsproj(string path) {
		string[] csprojContents = [
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
			"	</ItemGroup>",
			"",
			"</Project>"
		];

		File.WriteAllLines(path, csprojContents, System.Text.Encoding.UTF8);
	}

	/// <summary>
	/// Creates a new .mod.json file
	/// </summary>
	/// <param name="modName"></param>
	/// <param name="author"></param>
	/// <param name="newModPath">Full file path with extension</param>
	public static void CreateModManifest(string modName, string author, string newModPath) {
		ModManifest modManifest = new ModManifest() {
			Name = modName,
			Author = author,
			Version = "1.0.0",
			ModAssembly = $"bin/Debug/net48/{modName}.dll",
			LoadAfter = []
		};

		File.WriteAllText(newModPath, JsonWriter.ToJson(modManifest).Prettify());
	}

	/// <summary>
	/// Creates a new .cs file for the independent mod
	/// </summary>
	/// <param name="modName"></param>
	/// <param name="path">Full file path with extension</param>
	public static void CreateModEntrypointFile(string modName, string path) {
		string[] modFileContent = [
			"using RL2.ModLoader;",
			"",
			$"namespace {modName};",
			"",
			"[ModEntrypoint]",
			$"public class {modName}",
			"{",
			$"	public {modName}() {{ }}",
			"}"
		];

		File.WriteAllLines(path, modFileContent, System.Text.Encoding.UTF8);
	}
}