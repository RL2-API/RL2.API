using RL2.ModLoader;
using System;
using UnityEngine;

namespace TestMod;

public class TestMod : Mod
{
    public override string Name { get => "TestMod"; }

    public Action<MonoBehaviour, EventArgs> ActionOnJump;
    public Action<MonoBehaviour, EventArgs> ActionOnAttack;

    public override void OnLoad()
    {
        ActionOnJump = OnJump;
        ActionOnAttack = OnAttack;
        Messenger<GameMessenger, GameEvent>.AddListener(GameEvent.PlayerJump, ActionOnJump);
        Messenger<GameMessenger, GameEvent>.AddListener(GameEvent.PlayerWeaponAbilityCast, ActionOnAttack);
        ModLoader.Log("Attached listener to GameEvent.PlayerJump");
    }

    public override void OnUnload()
    {
        Messenger<GameMessenger, GameEvent>.RemoveListener(GameEvent.PlayerJump, ActionOnJump);
        Messenger<GameMessenger, GameEvent>.RemoveListener(GameEvent.PlayerWeaponAbilityCast, ActionOnAttack);
        ModLoader.Log("OnUnload was ran!");
    }

    public void OnJump(MonoBehaviour sender, EventArgs eventArgs)
    {
        ModLoader.Log($"Player jumped, args: {eventArgs}");
    }

    public void OnAttack(MonoBehaviour sender, EventArgs eventArgs)
    {
        if (Input.GetKey(KeyCode.BackQuote))
        {
            Messenger<DebugMessenger, DebugEvent>.Broadcast(DebugEvent.ToggleFPSCounter, null, null);
        }
    }
}