using System;
using UnityEngine;

namespace TestMod.Systems;

public static class TestModSystem {
    public static void TestSystemRun(MonoBehaviour sender, EventArgs eventArgs)
    {
        TestMod.Log("TestSystemRan");
    }
}