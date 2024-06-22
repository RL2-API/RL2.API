using System.Collections;
using UnityEngine;

namespace RL2.ModLoader;

/// <summary>
/// A component representing the in game console
/// </summary>
public class Console : MonoBehaviour
{
	private bool visible = false;

	private uint consoleLines = 15;  // number of messages to keep
	private Queue logQueue = new Queue();
	private string command = string.Empty;
	private GUIStyle style = new GUIStyle() {
		normal = new GUIStyleState() {
			background = Texture2D.grayTexture
		} 
	};

	private void OnEnable() {
		Application.logMessageReceived += HandleLog;
	}

	private void OnDisable() {
		Application.logMessageReceived -= HandleLog;
	}

	private void HandleLog(string logString, string stackTrace, LogType type) {
		string message = logString;
		if (type == LogType.Exception) {
			message += "\n" + stackTrace;
		}
		foreach (string line in message.Split('\n')) {
			logQueue.Enqueue(line);
		}
		while (logQueue.Count > consoleLines) {
			logQueue.Dequeue();
		}
	}

	private void Update() { }

	private void OnGUI() {
		if (Event.current.type == EventType.KeyDown) {
			if (Event.current.keyCode == KeyCode.BackQuote) {
				visible = !visible;
			}
			if (Event.current.keyCode == KeyCode.Return) {
				if (command != string.Empty) {
					if (command[0] == '/') {
						CommandManager.RunCommand(command.Substring(1));
					}
					else {
						Debug.Log(command);
					}
				}
				command = string.Empty;
			}
			if (Event.current.keyCode == KeyCode.Escape && visible) {
				command = string.Empty;
				visible = false;
			}
		}

		if (visible) {
			GUILayout.BeginArea(new Rect(Screen.width * 0.02f, Screen.height - 395, Screen.width * 0.96f, 330), style);
			GUILayout.Label(string.Join("\n", logQueue.ToArray()));
			GUILayout.EndArea();

			GUILayout.BeginArea(new Rect(Screen.width * 0.02f, Screen.height - 25f, Screen.width * 0.96f, 20f));

			GUI.SetNextControlName("command");
			command = GUILayout.TextField(command);
			if (command.EndsWith("`")) {
				command = string.Empty;
			}

			GUILayout.EndArea();

			GUI.FocusControl("command");
		}
	}
}
