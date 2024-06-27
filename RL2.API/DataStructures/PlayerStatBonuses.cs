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
	/// <summary>
	///	Additional Dexterity scaling. 0.1f is 10%, 1f is 100% etc.
	/// </summary>
	public float CritDamage = 0;
	/// <summary>
	///	Additional crit chance. 0.1f is 10%, 1f is 100% etc.
	/// </summary>
	public float CritChance = 0;
	public float Dextrity = 0;
	/// <summary>
	///	Additional Focus scaling. 0.1f is 10%, 1f is 100% etc.
	/// </summary>
	public float MagicCritDamage = 0;
	/// <summary>
	///	Additional magic crit chance. 0.1f is 10%, 1f is 100% etc.
	/// </summary>
	public float MagicCritChance = 0;
	public float Focus = 0;
	/// <summary>
	/// Additive Resolve bonus. 0.1f is 10 resolve, 1f is 100 resolve etc.
	/// </summary>
	public float Resolve = 0;
	public float Strength = 0;
	public float Intelligence = 0;
	public float InvincibilityDuration = 0;

	/// <summary>
	/// Armor multiplier, 1 is 100%, 1.5 is 150% etc. <br/>
	/// Make sure to update <see cref="PlayerController.CurrentArmor"/> manually after changing to immedieately take effect.
	/// </summary>
	public float ArmorMultiplier = 1f;
	public float VitalityMultiplier = 0;
	public float ManaMultiplier = 1f;
	public float HealthMultiplier = 0;
	public float DextrityMultiplier = 0;
	public float FocusMultiplier = 0;
	public float ResolveMultiplier = 0;
	public float StrengthMultiplier = 0;
	public float IntelligenceMultiplier = 0;
	public float InvincibilityDurationMultiplier = 1f;

	public void Reset() {
		// Reset flat
		Armor = 0;
		Vitality = 0;
		Mana = 0;
		Health = 0;
		CritDamage = 0;
		CritChance = 0;
		Dextrity = 0;
		MagicCritDamage = 0;
		MagicCritChance = 0;
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
		DextrityMultiplier = 0;
		FocusMultiplier = 0;
		ResolveMultiplier = 0;
		StrengthMultiplier = 0;
		IntelligenceMultiplier = 0;
		InvincibilityDurationMultiplier = 0;
	}
}
#pragma warning restore 1591