using System;
using System.IO;
using System.Linq;

public class Program
{
	static readonly string ConfigPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "game-install-path.txt";
	static readonly string OutputLibPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase.Replace("RL2.ModLoader.Setup\\", "") + "source\\lib\\";

	public static void Main(string[] args) {
		if (!File.Exists(ConfigPath)) {
			Console.WriteLine("Provide your Rogue Legacy 2 installation path");
			string Path = Console.ReadLine();
			File.WriteAllText(ConfigPath, Path + "\\");
		}
		
		Console.WriteLine("\nStart copying libs...");
		string LibrariesLocation = File.ReadAllText(ConfigPath) + "Rogue Legacy 2_Data\\Managed\\";
		DirectoryInfo librariesDirectory = new DirectoryInfo(LibrariesLocation);
		foreach (FileInfo fileInfo in librariesDirectory.GetFiles("**.dll")) {
			Console.WriteLine("Copying " + fileInfo.Name + " to " + OutputLibPath + fileInfo.Name);
			File.Copy(fileInfo.FullName, OutputLibPath + fileInfo.Name, true);
		}
	}
}