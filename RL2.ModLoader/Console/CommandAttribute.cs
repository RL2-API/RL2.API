using System;

namespace RL2.ModLoader;

/// <summary>
/// Marks a method as a console command.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
	/// <summary>
	/// Block access to the parameterless constructor
	/// </summary>
	private CommandAttribute() { }	

	/// <summary>
	/// The name of the command
	/// </summary>
	public string CommandName;

	/// <summary>
	/// Register the method below as a command.
	/// </summary>
	/// <param name="name">The name of the command. Advised to use "ModName:CommandName" to avoid as many conflicts as possible</param>
	/// <remarks>The marked method must be <see langword="static"/>!</remarks>
	public CommandAttribute(string name) {
		CommandName = name;
	}
}
