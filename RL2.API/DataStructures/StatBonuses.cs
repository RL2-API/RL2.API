using RL2.API.DataStructures;

namespace RL2.API;

public struct StatBonuses {
	/// <summary> </summary>
	public Modifiers Armor;
	/// <summary> </summary>
	public Modifiers Vitality;
	/// <summary> </summary>
	public Modifiers Mana;
	/// <summary> </summary>
	public Modifiers Health;
	/// <summary> </summary>
	public Modifiers EquipmentWeight;
	/// <summary> </summary>
	public Modifiers RuneWeight;
	/// <summary>
	///	Additional crit chance. 0.1f is 10%, 1f is 100% etc.
	/// </summary>
	public Modifiers CritChance;
	/// <summary> </summary>
	public Modifiers Dextrity;
	/// <summary>
	///	Additional magic crit chance. 0.1f is 10%, 1f is 100% etc.
	/// </summary>
	public Modifiers MagicCritChance;
	/// <summary> </summary>
	public Modifiers Focus;
	/// <summary>
	/// Additive Resolve bonus. 0.1f is 10 resolve, 1f is 100 resolve etc.
	/// </summary>
	public Modifiers Resolve;
	/// <summary> </summary>
	public Modifiers Strength;
	/// <summary> </summary>
	public Modifiers Intelligence;
	/// <summary> </summary>
	public Modifiers InvincibilityDuration;
	/// <summary> </summary>
	public Modifiers DoubleJumps;
	/// <summary> </summary>
	public Modifiers Dashes;
}
