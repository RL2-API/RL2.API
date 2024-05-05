using System.Collections;
using System.IO;
using UnityEngine;

namespace RL2.ModLoader;

public class Console : MonoBehaviour
{
	private bool visible = false;

	uint consoleLines = 15;  // number of messages to keep
	Queue logQueue = new Queue();
	string command = string.Empty;
	GUIStyle style = new GUIStyle();

	void OnEnable() {
		Application.logMessageReceived += HandleLog;
	}

	void OnDisable() {
		Application.logMessageReceived -= HandleLog;
	}

	void HandleLog(string logString, string stackTrace, LogType type) {
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

	void Update() { }

	void OnGUI() {
		if (Event.current.type == EventType.KeyDown) {
			if (Event.current.keyCode == KeyCode.BackQuote) {
				visible = !visible;
			}
			if (Event.current.keyCode == KeyCode.Return) {
				CommandManager.RunCommand(command);
				command = string.Empty;
			}
			if (Event.current.keyCode == KeyCode.Escape && visible) {
				command = string.Empty;
				visible = false;
			}
		}

		if (visible) {
			// Set GUIStyle used
			style.normal.background = Texture2D.grayTexture;
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
