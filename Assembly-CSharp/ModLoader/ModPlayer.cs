using System;
using UnityEngine;

namespace RL2.ModLoader;

/// <summary>
/// Used to add functionality to the player character
/// </summary>
public abstract class ModPlayer : MonoBehaviour
{
	/// <summary>
	/// An instance of the PlayerController attached to the current player.
	/// </summary>
	public PlayerController PlayerController => PlayerManager.GetPlayerController();
	/// <summary>
	/// Ran immedieately after attaching the ModPlayer to the PlayerController.
	/// </summary>
	public virtual void OnLoad() { }
	/// <summary>
	/// Ran before the player dies, and before the death. Return <see langword="false"/> to prevent the player from dying.
	/// </summary>
	/// <param name="killer">The GameObject that killed the player</param>
	public virtual bool PreKill(GameObject killer) => true;
	/// <summary>
	/// Ran after the player dies.
	/// </summary>
	/// <param name="killer">The GameObject that killed the player</param>
	public virtual void OnKill(GameObject killer) { }
	/// <summary>
	/// Used to modify player stats.
	/// </summary>
	public virtual void ModifyStats() { }
	/// <summary>
	/// Responsible for saving modded data. TODO
	/// </summary>
	public virtual void SaveData() { }
	/// <summary>
	/// Responsible for loading modded data. TODO
	/// </summary>
	public virtual void LoadData() { }
	/// <summary>
	/// Determines wether the ModPlayer of this class should be attached to the player.
	/// </summary>
	public virtual bool IsLoadingEnabled() => true;
};