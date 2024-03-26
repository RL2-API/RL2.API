using System.Collections;
using UnityEngine;

namespace RL2.ModLoader;

public class Console : MonoBehaviour
{
    private bool visible = false;

    uint qsize = 15;  // number of messages to keep
    Queue myLogQueue = new Queue();

    string command = string.Empty;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLogQueue.Enqueue("[" + type + "] : " + logString);
        if (type == LogType.Exception)
        {
            myLogQueue.Enqueue(stackTrace);
        }
        while (myLogQueue.Count > qsize)
        {
            myLogQueue.Dequeue();
        }
    }

    void Update() { }

    void OnGUI()
    {
        if (Event.current.type == EventType.KeyDown)
        {
            if (Event.current.keyCode == KeyCode.BackQuote)
            {
                visible = !visible;
            }
            if (Event.current.keyCode == KeyCode.Return)
            {
                CommandManager.RunCommand(command);
                command = string.Empty;
            }
        }

        if (visible)
        {
            GUILayout.BeginArea(new Rect(Screen.width * 0.05f, (Screen.height - 15 * (myLogQueue.ToArray().Length + 5)), Screen.width * 0.9f, Screen.height * 0.9f));

            GUILayout.Label("\n" + string.Join("\n", myLogQueue.ToArray()));

            GUI.SetNextControlName("command");
            command = GUILayout.TextField(command);
            if (command.EndsWith("`"))
            {
                command = string.Empty;
            }
            GUILayout.EndArea();

            GUI.FocusControl("command");
        }
    }
}