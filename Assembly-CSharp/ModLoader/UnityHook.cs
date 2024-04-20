using UnityEngine;

namespace RL2.ModLoader;

public class UnityHook : MonoBehaviour
{
	public static Console Console = new Console();

	public void Awake() {
		DontDestroyOnLoad(this);
		gameObject.AddComponent<Console>();
	}
}
