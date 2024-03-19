using System;
using UnityEngine;

namespace RL2.ModLoader;

public class InputReader
{ 
	public void CheckInput()
	{
		foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
		{
			if (Input.GetKey(key))
			{
				Messenger<InputMessenger, KeyCode>.Broadcast(key, null, null);
			}
		}
	}
}