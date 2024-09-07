# RL2.API Changelog

## v0.2.0
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