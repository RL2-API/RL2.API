namespace RL2.ModLoader;

public abstract class Mod
{
    public abstract string Name { get; }

    public virtual void OnLoad() { }
    public virtual void OnUnload() { }
}