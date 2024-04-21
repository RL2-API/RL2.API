using System;
using UnityEngine;

namespace RL2.ModLoader;

public abstract class ModPlayer : MonoBehaviour
{
	public virtual void OnLoad() { }
	public virtual void OnPlayerDeath(GameObject killer) { }
	public virtual void OnRelicChanged(RelicType relicType) { }
	public virtual void SaveData() { }
	public virtual void LoadData() { }
	public virtual bool IsLoadingEnabled() => true;
};