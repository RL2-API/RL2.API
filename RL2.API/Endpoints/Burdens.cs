using Reflect = System.Reflection;
using Collections = System.Collections.Generic;
using MM = MonoMod.RuntimeDetour;
using Cil = MonoMod.Cil;
using IL = Mono.Cecil.Cil;

namespace RL2.API;

/// <summary> </summary>
public static class Burdens {
	internal static MM.IDetour[] Hooks = [
		SilenceMissingDataLogs!,
		ModifyData.Hook,
	];

	internal static Collections.Dictionary<BurdenType, BurdenData> ModdedStore = [];

	/// <summary> </summary>
	public static class ModifyData
	{
		/// <summary> </summary>
		public delegate void Definition(BurdenType type, BurdenData data);

		/// <summary> </summary>
		public static event Definition? Event;

		internal static MM.Hook Hook = new MM.Hook(
			typeof(BurdenLibrary).GetMethod(nameof(BurdenLibrary.GetBurdenData), Reflect.BindingFlags.Public | Reflect.BindingFlags.Static),
			Method,
			new MM.HookConfig() {
				ID = "RL2.API::Burdens.ModifyData",
				ManualApply = true,
			}
		);

		internal static BurdenData Method(System.Func<BurdenType, BurdenData> orig, BurdenType type) {
			var data = orig(type);
			if (data == null) {
				ModdedStore.TryGetValue(type, out data);
			}

			Event?.Invoke(type, data);
			return data;
		}
	}

	/// <summary>
	/// Allows extending the <see cref="BurdenType_RL.TypeArray"/>
	/// </summary>
	public static class ExtendTypeArray
	{
		/// <inheritdoc cref="ExtendTypeArray"/>
		/// <param name="list"></param>
		public delegate void Definition(ref Collections.List<BurdenType> list);

		/// <inheritdoc cref="Definition"/>
		public static event Definition? Event;

		internal static MM.Hook Hook = new MM.Hook(
			typeof(BurdenType_RL).GetProperty(nameof(BurdenType_RL.TypeArray), Reflect.BindingFlags.Public | Reflect.BindingFlags.Static).GetMethod,
			Method,
			new MM.HookConfig() {
				ID = "RL2.API::Burdens.ExtendTypeArray",
				ManualApply = true,
			}
		);

		internal static BurdenType[] Method(System.Func<BurdenType[]> orig) {
			var original = System.Linq.Enumerable.ToList(orig());
			original.AddRange(ModdedStore.Keys);
			Event?.Invoke(ref original);
			return original.ToArray();
		}
	}


	internal static MM.ILHook SilenceMissingDataLogs = new MM.ILHook(
		typeof(BurdenLibrary).GetMethod(nameof(BurdenLibrary.GetBurdenData), Reflect.BindingFlags.Public | Reflect.BindingFlags.Static),
		SilenceMissingDataLogsPatch,
		new MM.ILHookConfig() {
			ID = "RL2.API::IL::Burdens.",
			ManualApply = true,
		}
	);

	internal static void SilenceMissingDataLogsPatch(Cil.ILContext il) {
		var cursor = new Cil.ILCursor(il);

		if (cursor.TryGotoNext(Cil.MoveType.Before, i => Cil.ILPatternMatchingExt.MatchBrfalse(i, out _))) {
			cursor.Emit(IL.OpCodes.Ldarg_0);
			cursor.EmitDelegate(ModdedStore.ContainsKey);

			cursor.Emit(IL.OpCodes.Or);
		}
	}
}