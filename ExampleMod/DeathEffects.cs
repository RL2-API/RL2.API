using RL2.ModLoader;
using RL2.ModLoader.API;
using UnityEngine;

namespace ExampleMod;

public class DeathEffects : IRegistrable
{
	public void Register() {
		Player.OnKill += (PlayerController player, GameObject killer) => {
			TextPopupManager.DisplayTextDefaultPos(TextPopupType.GoldCollected, "YOU DIED", player, attachToSource: true);
		};
		Enemy.OnKill += (EnemyController enemy, GameObject killer) => {
			TextPopupManager.DisplayTextDefaultPos(TextPopupType.Interact, "PERISH!", enemy, attachToSource: true);
		};
	}
}