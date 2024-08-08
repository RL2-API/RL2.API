# RL2.API Changelog

## v0.2.0 (UNRELEASED)
### Bug fixes:
- StatBonuses.(Strength|StrengthMultiplier|Intelligence|IntelligenceMultilplier) no longer apply to enemies
- RL2.API no longer tries to load mods that don't specify being loaded after it
### Changes:
- `new-mod` command is now called via `rl2.api:new-mod`
### Additions:
- ModSystem.ModifyAbilityData:
	- Allows modifying AbilityData depending on the provided AbilityType
- ModSystem.ModifyEnemyClassData:
	- Allows modifying EnemyClassData depending on the provided EnemyType
	- By extent, allows for modifying changing EnemyData and AI scripts depending on EnemyRank