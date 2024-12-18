using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RL2.API;

public partial class RL2API {
	internal static Hook ModiftRelicData_Hook = new Hook(
		typeof(RelicLibrary).GetMethod("GetRelicData", BindingFlags.Public | BindingFlags.Static),
		(Func<RelicType, RelicData> orig, RelicType type) => {
			RelicData data = orig(type);
			if (data == null) {
				data = Relics.CustomRelicStore[(int)type];
			}
			Relics.ModifyData_Invoke(type, data);
			return data;
		},
		new HookConfig() {
			ID = "RL2.API::ModifyRelicData"
		}
	);

	internal static Hook RelicTypeArrayExtension_hook = new Hook(
		typeof(RelicType_RL).GetProperty("TypeArray", BindingFlags.Public | BindingFlags.Static).GetGetMethod(),
		(Func<RelicType[]> orig) => {
			List<RelicType> original = orig().ToList();
			original.AddRange(Relics.CustomRelicStore.Keys.Cast<RelicType>());
			return original.ToArray();
		},
		new HookConfig() {
			ID = "RL2.API::ExtendRelicTypeArray"
		}
	);

	internal static Hook ApplyRelic_Hook = new Hook(
		typeof(RelicObj).GetMethod("ApplyRelic", BindingFlags.Public | BindingFlags.Instance),
		static (Action<RelicObj, int> orig, RelicObj self, int levelChange) => {
			orig(self, levelChange);
			Relics.ApplyRelic_Invoke(self.RelicType, levelChange);
		},
		new HookConfig() {
			ID = "RL2.API::ApplyRelic"
		}
	);
}