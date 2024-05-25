# Changelog

# v0.0.1-alpha-3
- Added ModSystem.ModifyRoomIcon.
	- Allows to set a map icon for a room that is passed in as the argument.
- XML documentation is now generated on build and provided alongside the release.

# v0.0.1-alpha-2
- Fixes to `generate-mod-skeleton`.
	- The generated `*.csproj` file is now valid.
	- Generated `*.mod.json` file is now using pretty JSON format and follows symantic versioning rules.
	- Change default path to mod assembly to default VS2022 path (`bin\Debug\net40\*.dll`).
- Added warning in logs when an assembly sepcified in a `*.mod.json` file doesn't exist.
- Fixed a bug during mod loading that could set the Mod.Path variable to incorrect value.
- Fixed PreKill - now returning true causes the entity to stay at 1HP.
- Added SwapTexture method.
	- Takes in an instance of the texture you want to change on the enemy and a replacement, then it performs the swap.

# v0.0.1-alpha
- Initial release