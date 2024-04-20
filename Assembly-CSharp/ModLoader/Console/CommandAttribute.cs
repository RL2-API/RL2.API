using System;

namespace RL2.ModLoader;

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
	public string commandName;

	public CommandAttribute(string name) {
		commandName = name;
	}
}
