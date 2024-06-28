using System;
using System.IO;
using System.Threading;
using NativeFileDialogSharp;

namespace RL2.ModLoader.Installer;

public class Program
{
	static string CurrentPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

	[STAThread]
	static void Main(string[] args) {
		Console.WriteLine("Welcome to the RL2.ModLoader installer");
		Console.WriteLine("Choose your games installation directory");
		Thread.Sleep(500);

		DialogResult result = Dialog.FolderPicker();
		string DataPath = result.Path + "\\Rogue Legacy 2_Data";
		string ManagedPath = DataPath + "\\Managed";

		if (!Directory.Exists(DataPath) || !Directory.Exists(ManagedPath)) {
			Console.WriteLine("The provided path is incorrect");
			return;
		}

		// Copy .json files
		Copy("RuntimeInitializeOnLoads.json", DataPath);
		Copy("ScriptingAssemblies.json", DataPath);

		// Copy RL2.API assemblies
		Copy("RL2.ModLoader.xml", ManagedPath);
		Copy("RL2.ModLoader.dll", ManagedPath);
		Copy("RL2.ModLoader.pdb", ManagedPath);
		Copy("RL2.API.dll", ManagedPath);
		Copy("RL2.API.xml", ManagedPath);
		Copy("RL2.API.pdb", ManagedPath);

		// Copy MonoMod files
		Copy("Mono.Cecil.dll", ManagedPath);
		Copy("Mono.Cecil.Rocks.dll", ManagedPath);
		Copy("MonoMod.Common.dll", ManagedPath);
		Copy("MonoMod.Common.xml", ManagedPath);
		Copy("MonoMod.RuntimeDetour.dll", ManagedPath);
		Copy("MonoMod.RuntimeDetour.xml", ManagedPath);
		Copy("MonoMod.Utils.dll", ManagedPath);
		Copy("MonoMod.Utils.xml", ManagedPath);

		Console.WriteLine("\n\nInstallation complete. Press any key to exit...");
		Console.ReadKey();
	}

	private static void Copy(string sourceFile, string destPath)
	{
        File.Copy(CurrentPath + "//" + sourceFile, destPath + "//" +  sourceFile, true);
        Console.Write("|");
		Thread.Sleep(10);
    }
}
