using System;

/// <summary>
/// Marks the independant mods main entrypoint<br/>
/// Used on the class. Initialization of the class is handled by a parameterleess constructor
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ModEntrypointAttribute : Attribute
{
	/// <summary>
	/// <inheritdoc cref="ModEntrypointAttribute"/>
	/// </summary>
	public ModEntrypointAttribute() { }
}