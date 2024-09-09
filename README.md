# RL2.API
[![GitHub Release](https://img.shields.io/github/v/release/RL2-API/RL2.API.svg?logo=github&style=flat-square)](https://github.com/RL2-API/RL2.API/releases/latest)
![Docs](https://img.shields.io/badge/Documentation-Offline-orange?logo=github&style=flat-square)
[![Website](https://img.shields.io/badge/Website-gray?logo=webtrees&logoColor=white&style=flat-square)](https://rl2-modloader.onrender.com/mods/RL2.API)
![RL2.API Logo](https://raw.githubusercontent.com/RL2-API/RL2.ModLoader/main/Assets/ModLoaderIcon-NoText-700x700.png)


RL2.API is a modding API for [Rogue Legacy 2](https://roguelegacy2.com). It requires [RL2.ModLoader](https://github.com/RL2-API/RL2.ModLoader) to function.


## Installation
1. Follow the installation guide for [RL2.ModLoader](https://github.com/RL2-API/RL2.ModLoader)
2. Download the [latest release of RL2.API](https://github.com/RL2-API/RL2.API/releases/latest)
3. Unpack the `.zip` into the `GameInstallation\Rogue Legacy 2_Data\Mods` directory
4. Run the game once to let the mod loader recognise the mod

## Build from source
1. Run `git clone https://github.com/RL2-API/RL2.API --recurse-submodules`
2. Go to `RL2.ModLoader.DevSetup` and run the provided `.exe`
3. Choose your games installation directory
4. Open the solution and build it OR run `dotnet build -c Release`
5. Go to the output directory and copy the `RL2.API` directory from there into `GameInstallation\Rogue Legacy 2_Data\Mods`
6. Run the game once to let the mod loader recognise the mod

## Using RL2.API as a dependency
1. Obtain `RL2.API.dll` and `RL2.API.xml` by following either the "Installation" or "Build from source" section.
2. Add it as a reference in your `.csproj` like this:
```xml
<PropertyGroup>
	<Reference Include="YourPath\To\RL2.API.dll" />
<PropertyGroup>
```