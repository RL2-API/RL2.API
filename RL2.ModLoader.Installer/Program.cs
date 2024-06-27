using System;
using System.IO;
using System.Threading;
using NativeFileDialogSharp;

namespace RL2.ModLoader.Installer;

public class Program
{
	[STAThread]
	static void Main(string[] args) {
		Console.WriteLine("Welcome to the RL2.ModLoader installer");
		Console.WriteLine("Choose your games installation directory");
		Thread.Sleep(500);

		DialogResult result = Dialog.FolderPicker();
		string DataPath = result.Path + "\\Rogue Legacy 2_Data";
		string ManagedPath = DataPath + "\\Managed";
		string CurrentPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

		if (!Directory.Exists(DataPath) || !Directory.Exists(ManagedPath)) {
			Console.WriteLine("The provided path is incorrect");
			return;
		}

		// Copy .json files
		Copy(CurrentPath + "\\RuntimeInitializeOnLoads.json", DataPath + "\\RuntimeInitializeOnLoads.json");
		Copy(CurrentPath + "\\ScriptingAssemblies.json", DataPath + "\\ScriptingAssemblies.json");

		// Copy RL2.API assemblies
		Copy(CurrentPath + "\\RL2.ModLoader.dll", ManagedPath + "\\RL2.ModLoader.dll");
		Copy(CurrentPath + "\\RL2.ModLoader.xml", ManagedPath + "\\RL2.ModLoader.xml");
		Copy(CurrentPath + "\\RL2.ModLoader.pdb", ManagedPath + "\\RL2.ModLoader.pdb");
		Copy(CurrentPath + "\\RL2.API.dll", ManagedPath + "\\RL2.API.dll");
		Copy(CurrentPath + "\\RL2.API.xml", ManagedPath + "\\RL2.API.xml");
		Copy(CurrentPath + "\\RL2.API.pdb", ManagedPath + "\\RL2.API.pdb");

		// Copy MonoMod files
		Copy(CurrentPath + "\\Mono.Cecil.dll", ManagedPath + "\\Mono.Cecil.dll");
		Copy(CurrentPath + "\\Mono.Cecil.Rocks.dll", ManagedPath + "\\Mono.Cecil.Rocks.dll");
		Copy(CurrentPath + "\\MonoMod.Common.dll", ManagedPath + "\\MonoMod.Common.dll");
		Copy(CurrentPath + "\\MonoMod.Common.xml", ManagedPath + "\\MonoMod.Common.xml");
		Copy(CurrentPath + "\\MonoMod.RuntimeDetour.dll", ManagedPath + "\\MonoMod.RuntimeDetour.dll");
		Copy(CurrentPath + "\\MonoMod.RuntimeDetour.xml", ManagedPath + "\\MonoMod.RuntimeDetour.xml");
		Copy(CurrentPath + "\\MonoMod.Utils.dll", ManagedPath + "\\MonoMod.Utils.dll");
		Copy(CurrentPath + "\\MonoMod.Utils.xml", ManagedPath + "\\MonoMod.Utils.xml");

		Console.WriteLine("\nInstallation complete. Press any key to exit...");
		Console.ReadKey();
	}

	private static void Copy(string sourcePath, string destPath)
	{
        File.Copy(sourcePath, destPath, true);
        Console.Write("|");
		Thread.Sleep(10);
    }
}
