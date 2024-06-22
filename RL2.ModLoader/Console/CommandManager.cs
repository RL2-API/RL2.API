using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RL2.ModLoader;

/// <summary>
/// Manages all commands
/// </summary>
public static class CommandManager
{
	/// <summary>
	/// Stores all commands
	/// </summary>
	public static Dictionary<string, MethodInfo> Commands = new Dictionary<string, MethodInfo>();

	/// <summary>
	/// Register all commands from the provided assembly.
	/// </summary>
	/// <param name="assembly">The assembly to be scanned for commands</param>
	public static void RegisterCommands(Assembly assembly) {
		MethodInfo[] methods = assembly.GetTypes()
					  .SelectMany(t => t.GetMethods())
					  .Where(m => m.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0)
					  .Where(m => m.IsStatic)
					  .ToArray();

		foreach (MethodInfo method in methods) {
			CommandAttribute command = method.GetCustomAttribute<CommandAttribute>(true);

			Commands.TryGetValue(command.CommandName, out MethodInfo alreadyExists);
			if (alreadyExists is not null) {
				ModLoader.Log($"Command with name {command.CommandName} already exists");
				return;
			}
			ModLoader.Log($"Found {command.CommandName} command");
			Commands.Add(command.CommandName, method);
		}
	}

	/// <summary>
	/// Runs the command with the provided arguments
	/// </summary>
	/// <param name="command">string representing the command and it's arguments</param>
	public static void RunCommand(string command) {
		string[] args = command.Split(' ');
		if (!Commands.TryGetValue(args[0], out MethodInfo commandMethod)) {
			ModLoader.Log($"Command with name {args[0]} not found");
			return;
		}
		commandMethod.Invoke(commandMethod.DeclaringType, new object[] { args.Skip(1).ToArray() });
	}
}
