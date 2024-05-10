using UnityEngine;

namespace RL2.ModLoader;

/// <summary>
/// Constantly active script attached to the <see cref="GameManager"/>'s GameObject.
/// </summary>
public abstract class ModSystem : ModType
{
	/// <summary>
	/// Ran immedieately after loading the ModSystem.
	/// </summary>
	public virtual void OnLoad() { }
	
	/// <summary>
	/// Determines whether the instance of this class should be initialized.
	/// </summary>
	public virtual bool IsLoadingEnabled() => true;
}
