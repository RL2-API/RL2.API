using UnityEngine;

namespace RL2.ModLoader;

public abstract class ModPlayer : MonoBehaviour
{
    public PlayerController Player => PlayerManager.GetPlayerController();

    public virtual void ResetEffects() { }

    /// <summary>
    /// Ran before every Update
    /// </summary>
    public virtual void PreUpdate() { }

    /// <summary>
    /// Ran at the end of LateUpdate()
    /// </summary>
    public virtual void PostUpdate() { }

    /// <summary>
    /// Use this to modify player stats
    /// </summary>
    public virtual void ModifyStats() { }

    /// <summary>
    /// TODO: Decide how should we approach saving
    /// </summary>
    //public virtual void SaveData() { }
    
    //public virtual void LoadData() { }
}