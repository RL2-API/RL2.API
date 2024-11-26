# RL2.API Changelog

## v1.1.1
### Fixes:
- Bad method signature in register content hook

## v1.1.0
### Changes:
- RL2.API now requires RL2.ModLoader v1.0.3
  - This is due to the fact that v1.0.3 introduced proper IDs for its Hook objects

### Additions:
- `Mod.ModManifest` property
  - Holds the `ModManifest` object related to the mod
- `RL2API.GetModInstance(string modName)` method
  - Returns an instance of a mod with `modName` set as its name in its manifest, or `null` if not found
- Mods can now specify which classes implementing `IRegistrable` belong to which mod
  - This is important in case one assembly contains multiple mods

## v1.0.0
### Architecture rework:
- Instead of subclassing ModX and GlobalX classes, modders should now create their own classes implementing the `IRegistrable` interface.
- Inside the `IRegistrable.Register()` method of their classes, they should subscribe to events such as `Player.OnKill` where their code will be executed

### Changes:
- Player related APIs
	- `ModPlayer.OnSpawn` override changed to
		```cs
		Player.OnSpawn += (PlayerController player) => { }
		```

	- `ModPlayer.PreKill` override changed to
		```cs
		Player.PreKill += (PlayerController player, GameObject killer) => { 
			return (bool)IfPlayerShouldDie;
		}
		```

	- `ModPlayer.OnKill` override changed to
		```cs
		Player.OnKill += (PlayerController player, GameObject killer) => { }
		```

	- Player stat modification procedurue changed. Instead of changing Player.StatBonuses.{StatName}, modders should use
		```cs
		Player.Stats.{StatName}Flat += (ref int/float additive) => { }
		```
		for additive bonuses, and 
		```cs
		Player.Stats.{StatName}Multiplier += (ref float multiplier) => { }
		```
		for stat scaling bonuses. Modify the passed in parameters to grant these bonuses

	- `ModSystem.ModifyGeneratedCharacterData` override changed to
		```cs
		Player.HeirGeneration.ModifyCharacterData += (CharacterData data, bool classLocked, bool spellLocked) => { }
		```

	- `ModSystem.ModifyGeneratedCharacterLook` override changed to
		```cs
		Player.HeirGeneration.ModifyCharacterLook += (PlayerLookController lookData, CharacterData data) => { }
		```

	- `ModSystem.ModifyCharacterRandomization` override changed to
		```cs
		Player.HeirGeneration.ModifyCharacterRandomization += (CharacterData data) => { }
		```
		
	- `ModSystem.ModifyAbilityData` override changed to
		```cs
		Player.Ability.ModifyData += (AbilityType type, AbilityData data) => { }
		```
- Enemy related APIs
	- `GlobalEnemy.OnSpawn` override changed to
		```cs
		Enemy.OnSpawn += (EnemyController enemy) => { }
		```
		
	- `GlobalEnemy.PreKill` override changed to
		```cs
		Enemy.PreKill += (EnemyController enemy, GameObject killer) => {
			return (bool)IfEnemyShouldDie;
		}
		```
		
	- `GlobalEnemy.OnKill` override changed to
		```cs
		Enemy.OnKill += (EnemyController enemy, GameObject killer) => { }
		```
		
	- `ModSystem.ModifyEnemyData` override changed to
		```cs
		Enemy.ModifyData += (EnemyType type, EnemyRank rank, EnemyData data) => { }
		```
		
	- `ModSystem.ModifyenemyBehaviour` override changed to
		```cs
		Enemy.ModifyBehaviour += (EnemyType type, EnemyRank rank, BaseAIScript aiScript, LogicController_SO logicController_SO) => { }
		```

- Map related APIs
	- `ModSystem.ModifyRoomIcon` override changed to
		```cs
		World.Map.ModifyRoomIcon += (GridPointManager roomToCheck, bool getUsed, bool isMergeRoom) => { }
		```

### Deletions
- ModClassData
	- Awaits new implemenataion, doesn't work at the moment

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