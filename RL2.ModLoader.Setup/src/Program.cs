using System;
using System.IO;

public class Program
{
	static readonly string ConfigPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config\\game-install-path.txt";
	static string OutputLibPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Replace("RL2.ModLoader.Setup\\", "") + "source\\lib\\";

	public static void Main(string[] args) {
		if (!File.Exists(ConfigPath)) {
			if (!Directory.Exists(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config")) {
				Directory.CreateDirectory(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "config");
			}
			Console.WriteLine("Provide your Rogue Legacy 2 installation path");
			string Path = Console.ReadLine();
			File.WriteAllText(ConfigPath, Path + "\\");
		}
		
		if (args != null) {
			if (args.Length != 0) {
				OutputLibPath = args[0] + "\\";
			}
		}

		if (!Directory.Exists(OutputLibPath)) {
			Directory.CreateDirectory(OutputLibPath);
		}

		Console.WriteLine("\nStart copying libs...");
		string LibrariesLocation = File.ReadAllText(ConfigPath) + "Rogue Legacy 2_Data\\Managed\\";
		DirectoryInfo librariesDirectory = new DirectoryInfo(LibrariesLocation);
		foreach (FileInfo fileInfo in librariesDirectory.GetFiles("**.dll")) {
			Console.WriteLine("Copying " + fileInfo.Name + " to " + OutputLibPath + fileInfo.Name);
			File.Copy(fileInfo.FullName, OutputLibPath + fileInfo.Name, true);
		}
		Console.WriteLine("Press any key to close...");
		Console.ReadKey();
	}
}