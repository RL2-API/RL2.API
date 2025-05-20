namespace RL2.API;

/// <summary>
/// Contains extensions for SummonRule types
/// </summary>
public static class X_SummonRule_Extensions {
	/// <summary>
	/// 
	/// </summary>
	/// <param name="self"></param>
	/// <param name="enemyPool"></param>
	/// <param name="isBiomeSpecific"></param>
	/// <param name="flyingOnly"></param>
	/// <returns></returns>
	public static SetSummonPool_SummonRule Set(this SetSummonPool_SummonRule self, EnemyTypeAndRank[]? enemyPool = null, bool? isBiomeSpecific = null, bool? flyingOnly = null) {
		if (enemyPool != null) self.m_enemiesToSummonArray = enemyPool;
		if (isBiomeSpecific is bool biomeSpecific) self.m_poolIsBiomeSpecific = biomeSpecific;
		if (flyingOnly is bool forceFlying) self.m_spawnFlyingOnly = forceFlying;
		return self;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="self"></param>
	/// <param name="level"></param>
	/// <param name="setToPlayerLevel"></param>
	/// <returns></returns>
	public static SetSummonPoolLevelMod_SummonRule Set(this SetSummonPoolLevelMod_SummonRule self, int? level = null, bool? setToPlayerLevel = null) {
		if (level is int lvl) self.m_levelMod = lvl;
		if (setToPlayerLevel is bool playerLevel) self.m_setLevelToRoom = playerLevel;
		return self;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="self"></param>
	/// <param name="summonValue"></param>
	/// <param name="summonDelay"></param>
	/// <param name="randomizeOnce"></param>
	/// <param name="spawnFast"></param>
	/// <param name="spawnAsCommander"></param>
	/// <returns></returns>
	public static SummonEnemy_SummonRule Set(this SummonEnemy_SummonRule self, float summonValue = 0f, float summonDelay = 0f, bool randomizeOnce = false, bool spawnFast = false, bool spawnAsCommander = false) {
		if (summonValue is float value) self.m_summonValue = value;
		if (summonDelay is float delay) self.m_summonDelay = delay;
		if (randomizeOnce is bool randomize) self.m_randomizeEnemiesOnce = randomize;
		if (spawnFast is bool fast) self.m_spawnFast = fast;
		if (spawnAsCommander is bool commander) self.m_summonAsCommander = commander;
		return self;
	}
}