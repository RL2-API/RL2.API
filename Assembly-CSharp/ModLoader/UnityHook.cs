using System;
using UnityEngine;


namespace RL2.ModLoader;

public class UnityHook : MonoBehaviour
{
    private static InputReader InputReader = new InputReader();

    public void Awake()
    {
        DontDestroyOnLoad(this);
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