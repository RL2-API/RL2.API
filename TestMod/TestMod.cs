using RL2.ModLoader;
using System;
using UnityEngine;
using TestMod.Systems;

namespace TestMod;

public class TestMod : Mod
{
    public override string Name { get => "TestMod"; }
    public override void Load()
    {
        Messenger<GameMessenger, GameEvent>.AddListener(GameEvent.PlayerJump, OnJump);
        Messenger<GameMessenger, GameEvent>.AddListener(GameEvent.PlayerWeaponAbilityCast, OnAttack);
        Messenger<ModMessenger, ModLoaderEvent>.AddListener(ModLoaderEvent.Load, OnLoad);
        Messenger<ModMessenger, ModLoaderEvent>.AddListener(ModLoaderEvent.Load, TestModSystem.TestSystemRun);
        Messenger<ModMessenger, ModLoaderEvent>.AddListener(ModLoaderEvent.Unload, OnUnload);
        Log($"{Name} loaded!");
    }

    public void OnLoad(MonoBehaviour sender, EventArgs eventArgs)
    {
        Log("Cawabunga");
    }

    public void OnUnload(MonoBehaviour sender, EventArgs eventArgs)
    {
        Messenger<GameMessenger, GameEvent>.RemoveListener(GameEvent.PlayerJump, OnJump);
        Messenger<GameMessenger, GameEvent>.RemoveListener(GameEvent.PlayerWeaponAbilityCast, OnAttack);
        Messenger<ModMessenger, ModLoaderEvent>.RemoveListener(ModLoaderEvent.Load, OnLoad);
        Messenger<ModMessenger, ModLoaderEvent>.RemoveListener(ModLoaderEvent.Load, TestModSystem.TestSystemRun);
        Messenger<ModMessenger, ModLoaderEvent>.RemoveListener(ModLoaderEvent.Unload, OnUnload);
        Log($"{Name} unloaded!");
    }

    public void OnJump(MonoBehaviour sender, EventArgs eventArgs)
    {
        Log($"Player jumped, args: {eventArgs}");
    }

    public void OnAttack(MonoBehaviour sender, EventArgs eventArgs)
    {
        if (Input.GetKey(KeyCode.BackQuote))
        {
            Messenger<DebugMessenger, DebugEvent>.Broadcast(DebugEvent.ToggleFPSCounter, null, null);
        }
    }

    public new static void Log(string message)
    {
        Debug.Log($"[TestMod]: {message}");
    }
}