namespace RL2.ModLoader;

#pragma warning disable 1591
public sealed class PlayerStatBonuses
{
	/// <summary>
	/// Flat armor bonus. Make sure to update <see cref="PlayerController.CurrentArmor"/> manually after changing to immedieately take effect.
	/// </summary>
	public int Armor = 0;
	public int Vitality = 0;
	public int Mana = 0;
	public int Health = 0;
	public int EquipmentWeight = 0;
	public int RuneWeight = 0;
	public float CritDamage = 0;
	public float CritChance = 0;
	public float Dextrity = 0;
	public float MagicCritDamage = 0;
	public float MagicCritChance = 0;
	public float Focus = 0;
	public float Resolve = 0;
	public float Strength = 0;
	public float Intelligence = 0;
	public float InvincibilityDuration = 0;

	/// <summary>
	/// Armor multiplier, 1 is 100%, 1.5 is 150% etc. <br></br>
	/// Make sure to update <see cref="PlayerController.CurrentArmor"/> manually after changing to immedieately take effect.
	/// </summary>
	public float ArmorMultiplier = 1f;
	public float VitalityMultiplier = 0;
	public float ManaMultiplier = 1f;
	public float HealthMultiplier = 0;
	public float CritDamageMultiplier = 1f;
	public float DextrityMultiplier = 0;
	public float MagicCritDamageMultiplier = 1f;
	public float FocusMultiplier = 0;
	public float ResolveMultiplier = 0;
	public float StrengthMultiplier = 0f;
	public float IntelligenceMultiplier = 0f;
	public float InvincibilityDurationMultiplier = 1f;

	public void Reset() {
		// Reset flat
		Armor = 0;
		Vitality = 0;
		Mana = 0;
		Health = 0;
		CritDamage = 0;
		Dextrity = 0;
		MagicCritDamage = 0;
		Focus = 0;
		Resolve = 0;
		Strength = 0;
		Intelligence = 0;
		InvincibilityDuration = 0;
		EquipmentWeight = 0;
		RuneWeight = 0;

		// Reset multipliers
		ArmorMultiplier = 1f;
		VitalityMultiplier = 0;
		ManaMultiplier = 1f;
		HealthMultiplier = 0;
		CritDamageMultiplier = 1f;
		DextrityMultiplier = 0;
		MagicCritDamageMultiplier = 1f;
		FocusMultiplier = 0;
		ResolveMultiplier = 0;
		StrengthMultiplier = 0f;
		IntelligenceMultiplier = 0f;
		InvincibilityDurationMultiplier = 0;
	}
}
#pragma warning restore 1591