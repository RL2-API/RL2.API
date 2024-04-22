using UnityEngine;

namespace RL2.ModLoader;

/// <summary>
/// Constantly active script attached to the <see cref="GameManager"/>'s GameObject.
/// </summary>
public abstract class ModSystem : MonoBehaviour
{
	/// <summary>
	/// Ran immedieately after loading the ModSystem.
	/// </summary>
	public virtual void OnLoad() { }
	/// <summary>
	/// Determines wether the ModPlayer of this class should be initialized.
	/// </summary>
	/// <returns></returns>
	public virtual bool IsLoadingEnabled() => true;
}
