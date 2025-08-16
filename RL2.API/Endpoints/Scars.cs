using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RL2.API;

/// <summary>
/// Provides endpoints for Scar challenge related APIs
/// </summary>
public static class Scars
{
	internal static IDetour[] Hooks = [
		ModifySummonRules.Hook,
	];

	internal static Dictionary<string, string> RuleNameToFullName = [];

	internal static string GetFullName(string namespaceQualifiedName) {
		if (RuleNameToFullName.TryGetValue(namespaceQualifiedName, out string fullName)) {
			return fullName;
		}
		if (namespaceQualifiedName.StartsWith("RL2.API")) return namespaceQualifiedName + RL2API.RL2API_FullTypeNameSuffix;
		return namespaceQualifiedName + RL2API.AssemblyCsharp_FullTypeNameSuffix;
	}

	/// <summary> Registers <typeparamref name="T"/> as a custom summon rule </summary>
	/// <typeparam name="T">Your rule's type</typeparam>
	public static void RegisterCustomRule<T>() {
		Type type = typeof(T);
		RuleNameToFullName[type.FullName] = type.AssemblyQualifiedName;
	}

	/// <inheritdoc cref="Definition"/>
	public static class ModifySummonRules {
		/// <summary>
		/// Allows modifying rule chains of Scar challenges
		/// </summary>
		/// <param name="challenge">Scar challenge type</param>
		/// <param name="rules">List of rules that get executed in the challenge</param>
		public delegate void Definition(ChallengeType challenge, ref List<BaseSummonRule> rules);

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static ILHook Hook = new ILHook(
			typeof(SummonRuleController).GetMethod("OnAfterDeserialize", BindingFlags.Public | BindingFlags.Instance),
			Method,
			new ILHookConfig() {
				ID = "RL2.API::Scars.ModifySummonRules",
				ManualApply	= true,
			}
		);
		internal static void Method(ILContext il) {
			ILCursor cursor = new ILCursor(il);

			if (!cursor.TryGotoNext(MoveType.After, ins => ins.MatchLdloc(3))) return;

			cursor.Remove();
			cursor.EmitDelegate((EnemySummonSerializedData serialized) => {
				return GetFullName(serialized.DataType);
			});

			if (!cursor.TryGotoNext(MoveType.Before, ins => ins.MatchRet())) return;

			cursor.Emit(OpCodes.Ldarg_0);
			cursor.EmitDelegate((SummonRuleController self) => {
				int ruleCount = self.SummonRuleArray.Length;
				if (ruleCount > 2 && self.SummonRuleArray[ruleCount - 2] is EndChallenge_SummonRule challenge) {
					List<BaseSummonRule> rules = self.SummonRuleArray.ToList();
					Event?.Invoke(challenge.m_challengeType, ref rules);
					self.SummonRuleArray = rules.ToArray();
				}
			});
		}
	}
}
