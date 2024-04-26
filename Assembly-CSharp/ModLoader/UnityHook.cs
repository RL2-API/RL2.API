using System;
using UnityEngine;

namespace RL2.ModLoader;

public class UnityHook : MonoBehaviour
{
	public void Awake() {
		DontDestroyOnLoad(this);
		gameObject.AddComponent<Console>();
		
		foreach (Mod mod in ModLoader.LoadedMods) {
			foreach (Type modSystem in mod.ModSystems) {
				ModSystem modSystemInstance = gameObject.AddComponent(modSystem) as ModSystem;
				if (!modSystemInstance.IsLoadingEnabled()) {
					Destroy(modSystemInstance);
					continue;
				}
				modSystemInstance.OnLoad();
			}
		}
	}
}
