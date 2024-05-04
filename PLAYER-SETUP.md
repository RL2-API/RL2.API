# Two ways
There are two ways of obtaining the modified Assembly-CSharp.dll file.

## 1. Come to the CDG Discord server
... and ping me in the `#rl2-modding` channel.

## 2. Selfsufficient
For this you will need
- Python 3.10 or newer, and on your system PATH
- .NET Framework 4.0

1. Download this repository, either with `git clone` or download the soure from this page.
2. Go into it's directory and launch your terminal  
3. Run `python tools/dev.py configure` and specify the aboslute path to your game's folder.
4. Run `python tools/dev.py decompile` and follow the instructions.
5. Run `dotnet build Assembly-CSharp\Assembly-CSharp.csproj`.
6. The compiled assembly should be located in the `bin\` directory, most probably in the `Debug\net40` subdirectory.

### If you come across any issues, come to the CDG Discord server and ping me in the `#rl2-modding` channel.