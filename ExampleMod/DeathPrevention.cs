using RL2.API;
using UnityEngine;

namespace ExampleMod;

public class DeathPrevention : IRegistrable
{
	void IRegistrable.Register() {
		// Player can't die in any way
		Player.PreKill += (PlayerController player, GameObject killer) => killer.TryGetComponent<EnemyController>(out _);

		// No Eyeball enemy can die
		Enemy.PreKill += (EnemyController enemy, GameObject killer) => {
			return enemy.EnemyType != EnemyType.Eyeball;
		};
	}
}