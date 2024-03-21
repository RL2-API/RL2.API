using System.Collections;
using UnityEngine;

namespace RL2.ModLoader;

public class Console : MonoBehaviour
{
    private bool visible = false;

    uint qsize = 15;  // number of messages to keep
    Queue myLogQueue = new Queue();

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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            visible = !visible;
        }
    }

    void OnGUI()
    {
        if (visible)
        {
            GUILayout.BeginArea(new Rect(Screen.width - 500, Screen.height, 500, 300));
            GUILayout.Label("\n" + string.Join("\n", myLogQueue.ToArray()));
            GUILayout.EndArea();
        }
    }
}