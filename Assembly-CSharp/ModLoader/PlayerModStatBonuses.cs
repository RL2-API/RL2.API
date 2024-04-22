namespace RL2.ModLoader;

public sealed class PlayerModStatBonuses
{
	public int armor = 0;
	public int vitality = 0;
	public float critDamage = 0;
	public float dextrity = 0;
	public float magicCritDamage = 0;
	public float focus = 0;
	public float resolve = 0;

	public float armorMultiplier = 0;
	public float vitalityMultiplier = 0;
	public float critDamageMultiplier = 0;
	public float dextrityMultiplier = 0;
	public float magicCritDamageMultiplier = 0;
	public float focusMultiplier = 0;
	public float resolveMultiplier = 0;

	public void Reset() {
		armor = 0;
		vitality = 0;
		critDamage = 0;
		dextrity = 0;
		magicCritDamage = 0;
		focus = 0;
		resolve = 0;
		armorMultiplier = 0;
		vitalityMultiplier = 0;
		critDamageMultiplier = 0;
		dextrityMultiplier = 0;
		magicCritDamageMultiplier = 0;
		focusMultiplier = 0;
		resolveMultiplier = 0;
	}
}