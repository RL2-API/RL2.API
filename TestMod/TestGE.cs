using System.IO;
using RL2.ModLoader;
using UnityEngine;

namespace TestMod;

public class TestGE : GlobalEnemy
{
	Texture texture;

	public override void OnSpawn()
	{
		if (SaveManager.PlayerSaveData.GoldCollected == 10_000) {
			texture = LoadTexture(ModLoader.ModPath + "\\Namaah.png");
		}
		else {
			texture = LoadTexture(ModLoader.ModPath + "\\Namaah-kopia.png");
		}
		
		foreach (Renderer renderer in Enemy.RendererArray)
		{
			foreach (string id in renderer.material.GetTexturePropertyNames()) {
				if (id != "_DiffuseTexture") {
					continue;
				}

				Texture oldTexture = renderer.material.GetTexture(id);
				if (oldTexture != null) {
					renderer.material.SetTexture(id, texture);
				}
			}
		}
	}

	public void PrintTexture(Texture2D texture, string fname) {
		RenderTexture tmp = RenderTexture.GetTemporary(texture.width,texture.height,0,RenderTextureFormat.Default,RenderTextureReadWrite.Linear);
		Graphics.Blit(texture, tmp);
		RenderTexture previous = RenderTexture.active;
		RenderTexture.active = tmp;
		Texture2D myTexture2D = new Texture2D(texture.width, texture.height);
		myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
		myTexture2D.Apply();
		RenderTexture.active = previous;
		RenderTexture.ReleaseTemporary(tmp);
		File.WriteAllBytes(fname,myTexture2D.EncodeToPNG());
	}

	public Texture2D LoadTexture(string fname)
	{
		Texture2D texture = new Texture2D(1, 1);
		if (!File.Exists(fname)) return texture;
		texture.LoadImage(File.ReadAllBytes(fname));
		//LoadImage automatically copies the texture to gpu, so texture.Apply is redundant.
		return texture;
	}
}