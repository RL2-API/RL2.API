using RL2.ModLoader;
using RL2.ModLoader.API;
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