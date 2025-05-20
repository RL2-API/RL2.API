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
}