# RL2.API Changelog

## v0.2.0 (UNRELEASED)
### Bug fixes:
- StatBonuses.(Strength|StrengthMultiplier|Intelligence|IntelligenceMultilplier) no longer apply to enemies
### Additions:
- ModSystem.ModifyAbilityData:
	- Allows modifying AbilityData depending on the provided AbilityType
- ModSystem.ModifyEnemyClassData:
	- Allows modifying EnemyClassData depending on the provided EnemyType
	- By extent, allows for modifying changing EnemyData and AI scripts depending on EnemyRank