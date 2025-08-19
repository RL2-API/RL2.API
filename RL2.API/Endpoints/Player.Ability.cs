using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using static RL2.API.Player;

namespace RL2.API;

public static partial class Player
{
	/// <summary>
	/// Stores events related to player abilities (weapons, spells, talents)
	/// </summary>
	public static class Ability
	{
		internal static IDetour[] Hooks = [
			ModifyData.Hook,
		];

		/// <summary>
		/// Used to modify ability data
		/// </summary>
		public static class ModifyData
		{
			/// <inheritdoc cref="ModifyData"/>
			/// <param name="type">The queried ability</param>
			/// <param name="data">Returned data of the ability</param>
			public delegate void Definition(AbilityType type, AbilityData data);

			/// <inheritdoc cref="Definition"/>
			public static event Definition? Event;

			internal static Hook Hook = new Hook(
				typeof(AbilityLibrary).GetMethod("GetAbility", BindingFlags.Public | BindingFlags.Static),
				Method,
				new HookConfig() {
					ID = "RL2.API::Player.Ability.ModifyData",
					ManualApply = true,
				}
			);

			internal static BaseAbility_RL? Method(Func<AbilityType, BaseAbility_RL> orig, AbilityType type) {
				BaseAbility_RL? ability = orig(type);
				if (ability == null) {
					return null;
				}

				Event?.Invoke(type, ability.AbilityData);
				return ability;
			}
		}
	}
}