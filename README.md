![Mod Loader Icon](https://raw.githubusercontent.com/TacoConKvass/RL2-ModLoader/main/Assets/ModLoaderSocialPreview-1600x516.png)
# RL2-ModLoader
A work-in-progress Rogue Legacy 2 mod loader and API

### [I want to play/make mods](https://github.com/TacoConKvass/RL2-ModLoader/blob/main/SETUP.md)
### [I want to contribute](https://github.com/TacoConKvass/RL2-ModLoader/blob/main/CONTRIBUTING.md)

## Where can I download mods?
- Right now mods made for the RL2-ModLoader are only distributed on the CDG Discord server in the `#mod-forums` channel.


# Latest changes:
## RL2.ModLoader v1.0.2
### Bug fixes:
- Commands are now loaded from standalone mods correctly
- ModManifest sorting
- Command invoking now works properly
### Changes:
- The mod loaders version is now displayed in a new line after the game version
- Entries specified in LoadAfter property of a mod manifest are now logged before mod loading
### Additions:
- Builtin `create-mod` command
	- Usage `/create-mod [ModName - required] [Author - optional]`
	- Creates base project files for an independent mod
- Builtin `show-mods` command
	- Usage `/show-mods`
	- Displays a list of all enabled mods and their versions
## RL2.API v0.2.0
### Bug fixes:
- StatBonuses.(Strength|StrengthMultiplier|Intelligence|IntelligenceMultilplier) no longer apply to enemies
- RL2.API no longer tries to load mods that don't specify being loaded after it
### Changes:
- `new-mod` command is now called via `rl2.api:new-mod`
### Additions:
- ModSystem.ModifyAbilityData:
	- Allows modifying AbilityData depending on the provided AbilityType
- ModSystem.ModifyEnemyData:
	- Allows modifying EnemyData depending on the provided EnemyType and EnemyRank
- ModSystem.ModifyEnemyBehaviour:
	- Allows modifying the enemy AI script and logic controller depending on the provided EnemyType and EnemyRank (needs detailed documentation)