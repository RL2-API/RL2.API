using MonoMod.RuntimeDetour;
using RL2.ModLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using RL2ModLoader = RL2.ModLoader.ModLoader;

namespace RL2.API;

/// <summary>
/// Main entrypoint for the RL2.API
/// </summary>
[ModEntrypoint]
public partial class RL2API
{
	/// <summary>
	/// All loaded mods using RL2.API
	/// </summary>
	public static Mod[] LoadedMods = [];

	/// <summary>
	/// Contains assembly - mod instance pairs
	/// </summary>
	public static Dictionary<Assembly, Mod> AssemblyToMod = [];

	/// <summary>
	/// Contains assembly - manifest pairs
	/// </summary>
	public static Dictionary<Assembly, ModManifest> AssemblyToManifest = [];

	/// <summary>
	/// RL2.API manifest
	/// </summary>
	public static ModManifest Manifest = new ModManifest();

	/// <summary>
	/// Initializing the API, woohoo
	/// </summary>
	public RL2API() {
		Log("Starting loading...");
		Manifest = RL2ModLoader.ModManifestToPath.Keys.FirstOrDefault(m => m.Name == "RL2.API");

		RL2ModLoader.OnLoad += Hooks.Apply;
		RL2ModLoader.OnUnload += Hooks.Undo;
		RL2ModLoader.OnUnload += Relics.SaveData.SaveModdedData;

		ModManifest[] api_dependent_manifests = ParseModManifests();
		Type[] mod_types = GetModTypes(api_dependent_manifests);
		LoadedMods = LoadMods(mod_types);

		RL2ModLoader.OnLoad += () => {
			foreach (Mod mod in LoadedMods)
				mod.RegisterContent();
		};
	}

	internal static ModManifest[] ParseModManifests() {
		return RL2ModLoader.ModManifestToPath.Keys.Where(
			manifest => !RL2ModLoader.ModList.Disabled.Contains(manifest.Name) && manifest.LoadAfter.Contains(Manifest.Name)
		).ToArray();
	}

	internal static Type[] GetModTypes(ModManifest[] manifests) {
		Dictionary<string, Type> mod_types = [];

		foreach (ModManifest manifest in manifests) {
			string name = manifest.Name;
			if (mod_types.TryGetValue(name, out _)) {
				Log($"A newer version of {name} was already loaded");
				continue;
			}

			if (!RL2ModLoader.ModList.Enabled.Contains(name)) {
				Log($"");
				RL2ModLoader.ModList.Enabled.Add(name);
			}

			string assembly_path = Path.Combine(RL2ModLoader.ModManifestToPath[manifest], manifest.ModAssembly);
			if (!File.Exists(assembly_path)) {
				Log($"Mod assembly {assembly_path} was not found");
				continue;
			}

			byte[] assembly_file = File.ReadAllBytes(assembly_path);
			Assembly found_assembly = null!;
			try {
				found_assembly = Assembly.Load(assembly_file);
			}
			catch (Exception ex) {
				Log($"Failed to load assembly {assembly_path}: {ex}");
				continue;
			}

			IEnumerable<Type> mod_types_found = found_assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Mod)));
			if (mod_types_found.Count() != 1) {
				Log($"{assembly_path} is not a a valid asssembly - A mod assembly must contain exactly 1 mod class");
				continue;
			}

			AssemblyToManifest[found_assembly] = manifest;
			mod_types.Add(name, mod_types_found.First());
		}
		return mod_types.Values.ToArray();
	}

	internal static Mod[] LoadMods(Type[] modTypes) {
		List<Mod> mods = [];
		foreach (Type mod_type in modTypes) {
			ModManifest manifest = AssemblyToManifest[mod_type.Assembly];
			Mod mod = null!;
			try {
				mod = (Mod)Activator.CreateInstance(mod_type);
			}
			catch (Exception ex) {
				Log($"Failed to load {manifest.Name}: {ex}");
				continue;
			}


			mod.Path = RL2ModLoader.ModManifestToPath[manifest];
			mod.Code = mod_type.Assembly;
			mod.Manifest = manifest;
			mod.RegistrableContent = mod.Code.GetTypes().Where(t => typeof(IRegistrable).IsAssignableFrom(t)).ToArray();

			CommandManager.RegisterCommands(mod.Code);
			RL2ModLoader.LoadedModNamesToVersions[manifest.Name] = manifest.SemVersion;
			AssemblyToMod[mod.Code] = mod;

			RL2ModLoader.OnLoad += mod.OnLoad;
			RL2ModLoader.OnUnload += mod.OnUnload;
			mods.Add(mod);
		}
		return mods.ToArray();
	}

	/// <summary>
	/// Retrieves the instance of the provided mod
	/// </summary>
	/// <param name="modName">The name of the mod you are searching for, specified in the mods manifest</param>
	/// <returns>
	/// Stored instance of found mod;
	/// <see langword="null"/> if not found
	/// </returns>
	public static Mod? GetModInstance(string modName) {
		foreach (Mod mod in LoadedMods) {
			if (mod.Manifest.Name == modName) {
				return mod;
			}
		}

		return null;
	}

	internal static void Log(object message) {
		UnityEngine.Debug.Log($"[RL2.API]: {message}");
	}
}