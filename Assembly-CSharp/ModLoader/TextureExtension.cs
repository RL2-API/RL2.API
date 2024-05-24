using System.IO;
using UnityEngine;

public static class TextureExtension
{
	public static Texture2D ConvertToReadable(this Texture2D texture) {
		RenderTexture tmp = RenderTexture.GetTemporary(texture.width, texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
		Graphics.Blit(texture, tmp);
		RenderTexture previous = RenderTexture.active;
		RenderTexture.active = tmp;
		Texture2D myTexture2D = new Texture2D(texture.width, texture.height);
		myTexture2D.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
		myTexture2D.Apply();
		RenderTexture.active = previous;
		RenderTexture.ReleaseTemporary(tmp);
		return myTexture2D;
	}

	public static Texture2D LoadTexture(string path) {
		Texture2D texture = new Texture2D(1, 1);
		if (!File.Exists(path)) {
			return texture;
		}
		texture.LoadImage(File.ReadAllBytes(path));
		return texture;
	}
}