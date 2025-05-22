using System;
using System.Collections.Generic;

namespace RL2.API;

/// <summary>
/// Provides endpoints for Scar challenge related APIs
/// </summary>
public static class Scars
{
	/// <summary>
	/// 
	/// </summary>
	public delegate void ModifySummonRules_delegate(ChallengeType challenge, ref List<BaseSummonRule> rules);

	/// <inheritdoc cref="ModifySummonRules_delegate"/>
	public static event ModifySummonRules_delegate? ModifySummonRules;

	internal static void ModifySummonRules_Invoke(ChallengeType challenge, ref List<BaseSummonRule> rules) {
		ModifySummonRules?.Invoke(challenge, ref rules);
	}

	/// <summary> Registers T as a custom summon rule </summary>
	/// <typeparam name="T">Your rule's type</typeparam>
	public static void RegisterCustomRule<T>() {
		Type type = typeof(T);
		RL2API.RuleNameToFullName[type.FullName] = type.AssemblyQualifiedName;
	}
}