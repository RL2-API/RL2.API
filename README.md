# RL2-ModLoader
A work-in-progress Rogue Legacy 2 mod loader and API

# Contributing to the mod loader
0. Make sure you have at least Python 3.10 installed and on your PATH
1. Fork this repository, clone it and go to the directory you cloned it into.
2. Open your terminal in the directory.
	- Make sure you have dotnet development tools installed.
3. Run `python tools/dev.py configure` and specify the aboslute path to your game's folder.
4. Run `python tools/dev.py decompile` and follow the instructions.
5. Once the decompilation is completed open the Assembly-CSharp.sln file and make your changes.
6. Once you are done with a change run `python tools/dev.py prepare-to-patch`
	- If you made any changes to things that are done in already existing patches, remove the concerned patches prior to running the command.
7. Run `git diff -p --no-index RL2-Source/EditedFile.cs Assembly-CSharp/EditedFile.cs > PatchFileName`
	- If you are editing an already existing patch, make sure the PatchFileName is the same as the old one.
8. After you are done with all your changes, commit your changes and push to your fork of the repository.
9. Make a pull request with your changes.