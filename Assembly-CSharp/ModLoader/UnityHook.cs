using System;
using UnityEngine;


namespace RL2.ModLoader;

public class UnityHook : MonoBehaviour
{
    public static InputReader InputReader = new InputReader();
    public static Console Console = new Console();

    public void Awake()
    {
        DontDestroyOnLoad(this);
        gameObejct.AddComponent<Console>();
    }

    public void FixedUpdate()
    {
        InputReader.CheckInput();
        Messenger<UnityMessenger, UnityHookEvent>.Broadcast(UnityHookEvent.FixedUpdate, null, null);
    }

    public void Update()
    {
        Messenger<UnityMessenger, UnityHookEvent>.Broadcast(UnityHookEvent.Update, null, null);
    }
}