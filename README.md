# RL2-ModLoader
A work-in-progress Rogue Legacy 2 mod loader and API

# Setting up the development environment
0. Make sure you have at least Python 3.10 installed and on your PATH
1. Fork this repository, clone it and go to the directory you cloned it into.
2. Open your terminal in the directory.
	- Make sure you have dotnet development tools installed.
3. Run `python tools/dev.py configure` and specify the aboslute path to your game's folder.
4. Run `python tools/dev.py decompile`.
5. Once the decompilation is completed open the Assembly-CSharp.sln file.
Now you are ready to contribute to the mod loader.
If you come across any issues please create an issue here on GitHub.
