using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace RL2.API;

public partial class RL2API
{
	internal static ILHook OnSummonRuleControllerDeserializationFinished_Hook = new ILHook(
		typeof(SummonRuleController).GetMethod("OnAfterDeserialize", BindingFlags.Public | BindingFlags.Instance),
		(ILContext il) => {
			ILCursor cursor = new(il);

			if (!cursor.TryGotoNext(MoveType.After, ins => ins.MatchLdloc(3))) return;

			cursor.Remove();
			cursor.EmitDelegate((EnemySummonSerializedData serialized) => {
				return serialized.DataType + AssemblyCsharp_FullTypeNameSuffix;
			});

			if (!cursor.TryGotoNext(MoveType.Before, ins => ins.MatchRet())) return;

			cursor.Emit(OpCodes.Ldarg_0);
			cursor.EmitDelegate((SummonRuleController self) => {
				int ruleCount = self.SummonRuleArray.Length;
				if (ruleCount > 2 && self.SummonRuleArray[ruleCount - 2] is EndChallenge_SummonRule challenge) {
					List<BaseSummonRule> rules = self.SummonRuleArray.ToList();
					Scars.ModifySummonRules_Invoke(challenge.m_challengeType, ref rules);
					self.SummonRuleArray = rules.ToArray();
				}
			});
		}
	);
}