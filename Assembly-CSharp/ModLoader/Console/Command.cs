using System;

namespace RL2.ModLoader;

[AttributeUsage(AttributeTargets.Method)]
public class Command : Attribute
{
    public string commandName;

    public Command(string name)
    {
        commandName = name;
    }
}