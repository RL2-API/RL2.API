namespace RL2.ModLoader;

public class ModInstance
{
    public Mod ModClassInstance;
    public ModSystem[] ModSystemInstances;

    public ModInstance(Mod modClassInstance, ModSystem[] modSystemInstances)
    {
        ModClassInstance = modClassInstance;
        ModSystemInstances = modSystemInstances;
    }
}