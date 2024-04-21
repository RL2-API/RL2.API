using System;
using UnityEngine;

namespace RL2.ModLoader;

public class UnityHook : MonoBehaviour
{
	public static Console Console = new Console();

	public void Awake() {
		DontDestroyOnLoad(this);
		gameObject.AddComponent<Console>();
		if (ModLoader.LoadedMods != null) {
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
		else {
			Debug.Log("ModLoader.LoadedMods is null");
		}
	}
}
