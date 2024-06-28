![Mod Loader Icon](https://raw.githubusercontent.com/TacoConKvass/RL2-ModLoader/main/Assets/ModLoaderSocialPreview-1600x516.png)
# RL2-ModLoader
A work-in-progress Rogue Legacy 2 mod loader and API

### [I want to play/make mods](https://github.com/TacoConKvass/RL2-ModLoader/blob/main/SETUP.md)
### [I want to contribute](https://github.com/TacoConKvass/RL2-ModLoader/blob/main/CONTRIBUTING.md)

## Where can I download mods?
- Right now mods made for the RL2-ModLoader are only distributed on the CDG Discord server in the `#mod-forums` channel.

## Latest changes:
# v0.1.0
- Separated the mod loader and API from th main games asembly.
	- This should allow for an easier compilation process for contributors/self-sufficeint modders.
- Split the mod loader into two projects: RL2.ModLoader and RL2.API.
	- This would allow modders to choose not to use the API, but create an alternate framework or not use one at all and hook everything themselves.
		- If a modder chooses not to use the API they get more freedom in how to structure their project, for a cost of potentially more work.
		- Mods not using RL2.API are related to as `not API compliant`.
	- Provided RL2.ModLoader.Setup for easier setup.
- All features ported to the new version
- **RL2.ModLaoder now works on both the Epic Games Store and the Steam version of the game**