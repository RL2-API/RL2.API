using RL2.ModLoader;
using RL2.ModLoader.API;

namespace ExampleMod;

public class HeirGeneratorChanges : IRegistrable 
{
	void IRegistrable.Register() {
		// All Contrarian heirs have cannons and Transmogrifier always rolls a cannon
		Player.HeirGeneration.ModifyCharacterRandomization += (CharacterData data) => {
			data.Weapon = AbilityType.CannonWeapon;
		};

		// All weapons look like arrows
		Player.HeirGeneration.ModifyCharacterLook += (PlayerLookController lookData, CharacterData data) => {
			lookData.CurrentWeaponGeo = lookData.ArrowGeo;
		};

		Player.HeirGeneration.ModifyCharacterData += (CharacterData data, bool classLocked, bool spellLocked) => {
			data.Name = "I have been edited :3";
		};
	}
}