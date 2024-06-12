# Contributing
## Prerequisites
- .NET Framework 4.8
- Visual Studio 2022 (not necessary, but advised)
- git

## How to setup the developer environment?
1. Fork this repository, clone it and go to the directory you cloned it into.
2. Go to RL2.ModLoader.Setup and run the provided executable.
	- It will ask for the path to your games copy, which the will be stored in the `config` subdirectory.
	- Alternatively, you can copy all `*.dll` files in the `Managed` directory of your game yourself.
3. Your development environment is ready.

## What after building the solution?
1. Copy all `.dll` and `.xml` files into your `Managed` directory.
2. Copy the `.json` files into `Rogue Legacy 2_Data`.
3. Now the game should run under the mod loader.