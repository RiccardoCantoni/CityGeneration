using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PhysicalMap : MonoBehaviour{

	private Texture2D mapTexture;
	private Renderer rend;
	private int xSize, ySize;


	public void init(int xSize, int ySize){
		rend = GetComponent<Renderer>();
		mapTexture = new Texture2D(xSize, ySize);
		mapTexture.filterMode = FilterMode.Point;
		rend.material.mainTexture = mapTexture;
	}

	public void draw(IMapRenderer mapRenderer){
        MeshRenderer mr = GetComponent<MeshRenderer>();
        mr.enabled = true;
		Color[] pix;
		pix = mapRenderer.getColors ();
		mapTexture.SetPixels(pix);
		mapTexture.Apply();
	}

    public void savePNG(string path, float scale)
    {
        Texture2D t = Resize(mapTexture, (int)(mapTexture.width * scale), (int)(mapTexture.height * scale));
        byte[] bytes = t.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
    }

    public static Texture2D Resize(Texture2D source, int newWidth, int newHeight)
    {
        source.filterMode = FilterMode.Point;
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
        rt.filterMode = FilterMode.Point;
        RenderTexture.active = rt;
        Graphics.Blit(source, rt);
        Texture2D nTex = new Texture2D(newWidth, newHeight);
        nTex.ReadPixels(new Rect(0, 0, newWidth, newWidth), 0, 0);
        nTex.Apply();
        RenderTexture.active = null;
        return nTex;
    }
}
