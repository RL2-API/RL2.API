# I want to create mods for the game.

## Prerequisites
- The modloader assembly in your game - follow one of the ways described [here](https://github.com/TacoConKvass/RL2-ModLoader/blob/main/PLAYER-SETUP.md)
- Visual Studio 2022
- .NET Framework 4

## Creating your first mod
- Open the game after repalcing the original Asembly-CSharp with the modloader one.
- Press ` to open the command console.
- Run `generate-mod-skeleton  YourModsName`
	- If you did this for the first time, it will create a `\ModSources` subdirectory in the `Rogue Legacy 2_Data` folder.
- Open `\ModSources\YourModsName`.
- Inside you will see 3 files:
	- `YourModsName.mod.json` - the manifest file of the mod. This file is essential for your mod to function. It contains information such as:
		- Mod name
		- Version
		- Which assembly is the main mod assembly
		- Which other assemblies, not found in the `\Manged` directory, are essential to the fuctioning of your mod
		- Which mods should be loaded before this one. This list should contain their names as specified in the referneced mods `.mod.json` file
	- `YourModName.csproj` - the C# project file. Specifies references to other assemblies and the target framework.
	- `YourModName.cs` - an empty Mod class. Every mod is require to have exactly one of these, or it will fail to load.
- Open the `.csproj` file and press `Build` or `ctrl + B` to build your project.
	- You can also open your terminal in the directory and run `dotnet build`.
- After the mod compiles, make sure that the `ModAssembly` entry in `YourModsName.mod.json` points to your built assembly, including the file extension.
	- Default output location for VS is `\bin\Debug\net40`, but you can move the `.dll` outside of there, just ensure that entry contains the relative path to it.
- Copy the `YourModsName` directory into the `Mods` folder.
	- You can remove the source code from the copy, but make sure that you leave the built assembly, dependecies and files that you use on runtime, then once again make sore that all entries in your `.mod.json` manifest have the correct relative path set.
- Run the game.
- If everything worked properly, you should be able to find a line in your `Player.log` that says `YourModName was loaded!`.