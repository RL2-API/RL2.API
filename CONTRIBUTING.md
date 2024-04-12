# How to contribute to the mod loader?
## Prerequisites
- Python version at least 3.10
- Visual Studio 2022
- git
- dotnet development tools

## How to setup the developer environment?
1. Fork this repository, clone it and go to the directory you cloned it into.
2. Open your terminal in the directory.
3. Run `python tools/dev.py configure` and specify the aboslute path to your game's folder.
4. Run `python tools/dev.py decompile` and follow the instructions.
5. Once the decompilation is completed open the Assembly-CSharp.sln file and make your changes.
6. Once you are done with a change run `python tools/dev.py prepare-to-patch`
	- If you made any changes to files that are already patched by the modloader, move them outside of the Patches directory.
7. Run `git diff -p --no-index RL2-Source/EditedFile.cs Assembly-CSharp/EditedFile.cs > EditedFileName.cs.patch`
8. After you are done with all your changes, commit your changes and push to your fork of the repository.
9. Make a pull request with your changes.