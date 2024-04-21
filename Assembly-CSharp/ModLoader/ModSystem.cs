using UnityEngine;

namespace RL2.ModLoader;

public abstract class ModSystem : MonoBehaviour
{
	public virtual void OnLoad() { }
	public virtual bool IsLoadingEnabled() => true;
}
