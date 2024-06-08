namespace RL2.ModLoader;

public class APIStore
{
	public Mod[] LoadedMods;

	public APIStore() {
		LoadedMods = LoadAPICompliantMods();
	}

	public Mod[] LoadAPICompliantMods() {
		return [];
	}
}