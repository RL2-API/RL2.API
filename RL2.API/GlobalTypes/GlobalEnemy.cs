using System;
using System.Collections.Generic;
using UnityEngine;

namespace RL2.ModLoader;

/// <summary>
/// Used to alter enemies. For creating new AIScript's use <see cref="BaseAIScript"/>, and change the enemy's script in here.
/// </summary>
public abstract class GlobalEnemy : GlobalType
{
	/// <summary>
	/// An EnemyController instance attached to this enemy.
	/// </summary>
	public EnemyController Enemy => gameObject.GetComponent<EnemyController>();

	/// <summary>
	/// The enemy type represented as an <see langword="int"/>.
	/// </summary>
	public int Type => (int)Enemy.EnemyType;

	/// <summary>
	/// The <see cref="EnemyRank"/> of the enemy.
	/// </summary>
	public EnemyRank Rank => Enemy.EnemyRank;

	/// <summary>
	/// Determines which enemies the instance of this GlobalEnemy will be attached to.<br/>
	/// Leave empty to attach to every enemy.
	/// </summary>
	public virtual Dictionary<int, EnemyRank[]> AppliesToEnemy => new Dictionary<int, EnemyRank[]>();

	/// <summary>
	/// Ran on enemy spawn.
	/// </summary>
	public virtual void OnSpawn() { }

	/// <summary>
	/// Determines wether the affected enemy should die.
	/// </summary>
	/// <param name="killer">GameObject responsible for the enemy's death</param>
	/// <returns>Whether the enemy should die.</returns>
	public virtual bool PreKill(GameObject killer) => true;

	/// <summary>
	/// Ran immedieately after the enemy dies.
	/// </summary>
	public virtual void OnKill(GameObject killer) { }

	/// <summary>
	/// Swaps a texture on all elements of the enemy's RendererArray.
	/// </summary>
	/// <param name="targetedTexture">The texture you want to change</param>
	/// <param name="newTexture">New texture</param>
	/// <param name="debug">Whether you want debug info in logs/console. Defaults to: <see langword="false"/></param>
	public void SwapTexture(Texture2D targetedTexture, Texture2D newTexture, bool debug = false) {
		if (debug) {
			ModLoader.Log("attempt texture swap");
		}
		foreach (Renderer renderer in Enemy.RendererArray) {
			foreach (string id in renderer.material.GetTexturePropertyNames()) {
				Texture2D currentTexture = (Texture2D)renderer.material.GetTexture(id);

				if (currentTexture == null) {
					continue;
				}
				if (currentTexture.width != targetedTexture.width || currentTexture.height != targetedTexture.height) {
					if (debug) {
						ModLoader.Log("Texture size doesn't match");
					}
					continue;
				}

				Color32[] origPixels = currentTexture.ConvertToReadable().GetPixels32();
				Color32[] checkedPixels = targetedTexture.GetPixels32();
				bool matchFailed = false;

				for (int i = 0; i < origPixels.Length; i += origPixels.Length / 20) {
					Color32 difference = new Color32(
						(byte)Math.Abs(checkedPixels[i].r - origPixels[i].r),
						(byte)Math.Abs(checkedPixels[i].g - origPixels[i].g),
						(byte)Math.Abs(checkedPixels[i].b - origPixels[i].b),
						(byte)Math.Abs(checkedPixels[i].a - origPixels[i].a)
					);
					if (difference.r > 5 || difference.g > 5 || difference.b > 5) {
						if (debug) {
							ModLoader.Log($"{checkedPixels[i]} != {origPixels[i]}");
						}
						matchFailed = true;
						break;
					}
				}

				if (matchFailed) {
					continue;
				}

				if (debug) {
					ModLoader.Log("All tests passed");
				}
				renderer.material.SetTexture(id, newTexture);
			}
		}
	}
}