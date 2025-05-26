using RL2.ModLoader;
using System.IO;
using System.Linq;
using UnityEngine;

namespace RL2.API;

/// <summary>
/// Store for all builtin RL2.API commands
/// </summary>
public class BuiltinCommands
{
	/// <summary>
	/// Creates a new mod
	/// </summary>
	/// <param name="args"></param>
	[Command("rl2.api:new-mod")]
	public static void NewMod(string[] args) {
		if (args.Length == 0) {
			RL2API.Log("\"rl2.api:new-mod\" command usage: /rl2.api:new-mod [modName - Required] [authors - Optional, Space separated list] ");
			return;
		}

		string name = args[0];
		string authors = args.Length == 1 ? "" : string.Join(", ", args.Skip(1));

		string newModPath = ModLoader.ModLoader.ModPath + $"\\{name}";
		if (Directory.Exists(newModPath)) {
			RL2API.Log($"A mod with this name: {name} already exists in your Mods directory");
			return;
		}

		newModPath = ModLoader.ModLoader.ModSources + $"\\{name}";
		if (Directory.Exists(newModPath)) {
			RL2API.Log($"A mod with this name: {name} already exists in your ModSources directory");
			return;
		}

		Directory.CreateDirectory(newModPath);
		newModPath += $"\\{name}";
		EnsureTargetsFiles();
		GenerateCsproj(newModPath);
		GenerateManifest(newModPath, name, authors);
		GenerateModFile(newModPath, name);
		RL2API.Log($"Mod {name} was created");
	}

	internal static void EnsureTargetsFiles() {
		ModLoader.ModLoader.EnsureTargetsFile();

		string targetsPath = ModLoader.ModLoader.ModSources + "\\RL2.API.targets";
		if (File.Exists(targetsPath)) return;

		ModManifest apiManifest = ModLoader.ModLoader.ModManifestToPath.Keys.Where(manifest => manifest.Name == "RL2.API").First();
		string RL2API_Path = ModLoader.ModLoader.ModManifestToPath[apiManifest].Replace("RL2.API.mod.json", "");
		File.WriteAllText(targetsPath,
			$"""
			<Project ToolsVersion="14.0" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
				<Import Project="RL2.Mods.targets" />

				<PropertyGroup>
					<RL2API_Path>{RL2API_Path}\</RL2API_Path>
				</PropertyGroup>

				<ItemGroup>
					<Reference Include="$(RL2API_Path)*.dll">
						<Private>false</Private>
					</Reference>
				</ItemGroup>
			</Project>
			"""
		);
	}

	internal static void GenerateCsproj(string newModPath) {
		string csprojContents = $"""
		<Project Sdk="Microsoft.NET.Sdk">
			<Import Project="../RL2.API.targets" />

			<PropertyGroup>
				<TargetFramework>net48</TargetFramework>
			</PropertyGroup>
		
			<ItemGroup>
			</ItemGroup>
		
		</Project>
		""";

		File.WriteAllText(newModPath + ".csproj", csprojContents);
	}

	internal static void GenerateManifest(string newModPath, string name, string authors) {
		ModManifest modManifest = new ModManifest() {
			Name = name,
			Author = authors,
			Version = "1.0.0",
			ModAssembly = $"{name}.dll",
			LoadAfter = ["RL2.API"]
		};
		File.WriteAllText(newModPath + ".mod.json", JsonUtility.ToJson(modManifest, true));
	}

	internal static void GenerateModFile(string newModPath, string name) {
		File.WriteAllText(newModPath + ".cs",
			$$"""
			using RL2.API;

			namespace {{name}};

			public class {{name}} : Mod
			{
				public override void OnLoad() {
					Mod.Log("{{name}} loaded!");
				}
			}
			"""
		);
	}
}