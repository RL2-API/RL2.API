using System.Collections.Generic;
using UnityEngine;

namespace RL2.ModLoader;

/// <summary>
/// Used to alter enemies. For creating new AIScript's use <see cref="BaseAIScript"/>, and change the enemy's script in here.
/// </summary>
public abstract class GlobalEnemy : ModType
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
	/// Determines which enemies the instance of this GlobalEnemy will be attached to.<br></br>
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
	/// <returns>Wether the enemy should die.</returns>
	public virtual bool PreKill(GameObject killer) => true;
	
	/// <summary>
	/// Ran immedieately after the enemy dies.
	/// </summary>
	public virtual void OnKill(GameObject killer) { }

	public void SwapTexture(Texture2D originalTexture, Texture2D newTexture) {
		ModLoader.Log("attempt texture swap");
		foreach (Renderer renderer in Enemy.RendererArray) {
			foreach (string id in renderer.material.GetTexturePropertyNames()) {
				Texture2D oldTexture = renderer.material.GetTexture(id) as Texture2D;
				
				if (oldTexture == null) {
					continue;
				}
				if (oldTexture.width != originalTexture.width || oldTexture.height != originalTexture.height) {
					ModLoader.Log("Texture size doesn't match");
					continue;
				}

				Color32[] origPixels = oldTexture.ConvertToReadable().GetPixels32();
				Color32[] checkedPixels = originalTexture.GetPixels32();
				bool matchFailed = false;

				for (int i = 0; i < origPixels.Length; i += origPixels.Length / 20) {	
					if (checkedPixels[i].r != origPixels[i].r || checkedPixels[i].g != origPixels[i].g || checkedPixels[i].b != origPixels[i].b) {
						ModLoader.Log($"{checkedPixels[i]} != {origPixels[i]}");
						matchFailed = true;
						break;
					}
				}

				if (matchFailed) {
					continue;
				}

				ModLoader.Log("Swappin'");
				renderer.material.SetTexture(id, newTexture);
			}
		}
	}
}