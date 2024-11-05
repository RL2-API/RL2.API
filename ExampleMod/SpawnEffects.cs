using RL2.API;

namespace ExampleMod;

public class SpawnEffects : IRegistrable 
{
	public void Register() {
		// Spawned players and enemies spawn a popup with text "I HAVE BEEN BORN!" 
		Player.OnSpawn += (PlayerController player) => {
			TextPopupManager.DisplayTextDefaultPos(TextPopupType.Interact, "I HAVE BEEN BORN!", player, attachToSource: true);
		};
		Enemy.OnSpawn += (EnemyController enemy) => {
			TextPopupManager.DisplayTextDefaultPos(TextPopupType.Interact, "I HAVE BEEN BORN!", enemy, attachToSource: true);
		};
	}
}