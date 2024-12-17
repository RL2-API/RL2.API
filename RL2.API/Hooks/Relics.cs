using MonoMod.RuntimeDetour;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RL2.API;

public partial class RL2API {
	internal static Hook ModiftRelicData_Hook = new Hook(
		typeof(RelicLibrary).GetMethod("GetRelicData", BindingFlags.Static | BindingFlags.Public),
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
		typeof(RelicType_RL).GetProperty("TypeArray", BindingFlags.Static | BindingFlags.Public).GetGetMethod(),
		(Func<RelicType[]> orig) => {
			List<RelicType> original = orig().ToList();
			original.AddRange(Relics.CustomRelicStore.Keys.Cast<RelicType>());
			return original.ToArray();
		},
		new HookConfig() {
			ID = "RL2.API::ExtendRelicTypeArray"
		}
	);
}