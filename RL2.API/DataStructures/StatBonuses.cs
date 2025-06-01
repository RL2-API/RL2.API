using RL2.API.DataStructures;

namespace RL2.API;

public struct Stats {
	public Modifiers Armor;
	public Modifiers Vitality;
	public Modifiers Mana;
	public Modifiers Health;
	public Modifiers EquipmentWeight;
	public Modifiers RuneWeight;
	/// <summary>
	///	Additional crit chance. 0.1f is 10%, 1f is 100% etc.
	/// </summary>
	public Modifiers CritChance;
	public Modifiers Dextrity;
	/// <summary>
	///	Additional magic crit chance. 0.1f is 10%, 1f is 100% etc.
	/// </summary>
	public Modifiers MagicCritChance;
	public Modifiers Focus;
	/// <summary>
	/// Additive Resolve bonus. 0.1f is 10 resolve, 1f is 100 resolve etc.
	/// </summary>
	public Modifiers Resolve;
	public Modifiers Strength;
	public Modifiers Intelligence;
	public Modifiers InvincibilityDuration;

	public Modifiers DoubleJumps;
	public Modifiers Dashes;
}
