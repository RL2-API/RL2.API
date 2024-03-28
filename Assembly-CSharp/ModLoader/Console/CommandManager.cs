using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RL2.ModLoader;

public static class CommandManager
{
	public static Dictionary<string, MethodInfo> commands = new Dictionary<string, MethodInfo>();

	public static void RegisterCommands(Assembly assembly)
	{
		MethodInfo[] methods = assembly.GetTypes()
					  .SelectMany(t => t.GetMethods())
					  .Where(m => m.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0)
					  .Where(m => m.IsStatic)
					  .ToArray();

		foreach (MethodInfo method in methods)
		{
			CommandAttribute command = method.GetCustomAttribute<CommandAttribute>(true);

			commands.TryGetValue(command.commandName, out MethodInfo alreadyExists);
			if (alreadyExists is not null)
			{
				ModLoader.Log($"Command with name {command.commandName} already exists");
				return;
			}
			ModLoader.Log($"Found {command.commandName} command");
			commands.Add(command.commandName, method);
		}
	}

	public static void RunCommand(string command)
	{
		string[] args = command.Split(' ');
		if (!commands.TryGetValue(args[0], out MethodInfo commandMethod))
			return;
		commandMethod.Invoke(commandMethod.DeclaringType, new object[] { args.Skip(1).ToArray()});
	}
}
