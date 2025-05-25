namespace RL2.API.DataStructures;

/// <summary>
/// Modifiers
/// </summary>
public class Modifiers()
{
	/// <summary>
	/// An additive bonus, applied before multiplicative bonuses.
	/// </summary>
	public float Additive = 0f;
	/// <summary>
	/// A multiplivcative bonus, applied after the additive bonuses.
	/// Increasing this by <code>0.1f</code> results in a 10% damage increase
	/// </summary>
	public float Multiplicative = 1f;
	/// <summary>
	/// Additional damage. Not affected by multipliers.
	/// </summary>
	public float Flat = 0f;
}