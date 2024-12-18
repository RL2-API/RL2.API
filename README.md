<h1 align="center">
    <img src="https://raw.githubusercontent.com/RL2-API/RL2.ModLoader/main/Assets/ModLoaderIcon-NoText-700x700.png" width=350px height=350px /><br/>
    RL2.API<br/>
    <a href="https://github.com/RL2-API/RL2.API/releases/latest"><img src="https://img.shields.io/github/v/release/RL2-API/RL2.API.svg?logo=github&style=flat-square" alt="Github Release"/></a>
    <a href=""><img src="https://img.shields.io/badge/Documentation-Offline-orange?logo=github&style=flat-square" alt="Docs"/></a>
    <a href="https://rl2-modloader.onrender.com/mods/RL2.API"><img src="https://img.shields.io/badge/Website-gray?logo=webtrees&logoColor=white&style=flat-square" alt="Website"/></a>
</h1>


RL2.API is a modding API for [Rogue Legacy 2](https://roguelegacy2.com). It requires [RL2.ModLoader](https://github.com/RL2-API/RL2.ModLoader) to function.


## Installation
1. Follow the installation guide for [RL2.ModLoader](https://github.com/RL2-API/RL2.ModLoader)
2. Download the [latest release of RL2.API](https://github.com/RL2-API/RL2.API/releases/latest)
3. Unpack the `.zip` into the `GameInstallation\Rogue Legacy 2_Data\Mods` directory
4. Run the game once to let the mod loader recognise the mod

## Build from source
1. Run `git clone https://github.com/RL2-API/RL2.API --recurse-submodules`
2. Go to `RL2.ModLoader.DevSetup` and run the provided `.exe` and choose your games installation directory
    - Alterantively, go to the `GameInstallation\Rogue Legacy 2_Data\Managed` dirtectory and copy all files into the repositories `lib` directory
3. Open the solution and build it OR run `dotnet build -c Release`
4. Go to the output directory and copy the `RL2.API` directory from there into `GameInstallation\Rogue Legacy 2_Data\Mods`
5. Run the game once to let the mod loader recognise the mod

## Using RL2.API as a dependency
1. Obtain `RL2.API.dll` and `RL2.API.xml` by following either the "Installation" or "Build from source" section.
2. Add it as a reference in your `.csproj` like this:
```xml
<PropertyGroup>
	<Reference Include="YourPath\To\RL2.API.dll" />
<PropertyGroup>
```
