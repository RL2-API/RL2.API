namespace RL2.ModLoader;

public sealed class PlayerModStatBonuses
{
	#region Flat bonuses
	/// <summary>
	/// Flat armor bonus. Make sure to update <see cref="PlayerController.CurrentArmor"/> manually after changing to immedieately take effect.
	/// </summary>
	public int armor = 0;
	public int vitality = 0;
	public int mana = 0;
	public int health = 0;
	public int equipmentWeight = 0;
	public int runeWeight = 0;
	public float critDamage = 0;
	public float dextrity = 0;
	public float magicCritDamage = 0;
	public float focus = 0;
	public float resolve = 0;
	public float strength = 0;
	public float intelligence = 0;
	public float invincibilityDuration = 0;
	#endregion

	#region Multipliers
	/// <summary>
	/// Armor multiplier, 1 is 100%, 1.5 is 150% etc. <br></br>
	/// Make sure to update <see cref="PlayerController.CurrentArmor"/> manually after changing to immedieately take effect.
	/// </summary>
	public float armorMultiplier = 1f;
	public float vitalityMultiplier = 0;
	public float manaMultiplier = 1f;
	public float healthMultiplier = 0;
	public float critDamageMultiplier = 1f;
	public float dextrityMultiplier = 0;
	public float magicCritDamageMultiplier = 1f;
	public float focusMultiplier = 0;
	public float resolveMultiplier = 0;
	public float strengthMultiplier = 1f;
	public float intelligenceMultiplier = 1f;
	public float invincibilityDurationMultiplier = 1f;
	#endregion

	public void Reset() {
		#region Reset flat
		armor = 0;
		vitality = 0;
		mana = 0;
		health = 0;
		critDamage = 0;
		dextrity = 0;
		magicCritDamage = 0;
		focus = 0;
		resolve = 0;
		strength = 0;
		intelligence = 0;
		invincibilityDuration = 0;
		equipmentWeight = 0;
		runeWeight = 0;
		#endregion

		#region Reset multipliers
		armorMultiplier = 1f;
		vitalityMultiplier = 0;
		manaMultiplier = 1f;
		healthMultiplier = 0;
		critDamageMultiplier = 1f;
		dextrityMultiplier = 0;
		magicCritDamageMultiplier = 1f;
		focusMultiplier = 0;
		resolveMultiplier = 0;
		strengthMultiplier = 1f;
		intelligenceMultiplier = 1f;
		invincibilityDurationMultiplier = 0;
		#endregion
	}
}