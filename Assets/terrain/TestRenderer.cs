using UnityEngine;
using UnityEditor;

public class TestRenderer : IMapRenderer
{
    Color[] buckets;
    float[,] heightMap;
    int xSize, ySize;

    public TestRenderer(float[,] heightMap, int xSize, int ySize)
    {
        this.heightMap = heightMap;
        this.xSize = xSize;
        this.ySize = ySize;
        buckets = new Color[]
        {
            new Color32(193,204,165, 0xff),
            new Color32(230,240,191, 0xff),
            new Color32(233,239,181, 0xff),
            new Color32(218,198,137, 0xff),
            new Color32(205,163,127, 0xff),
            new Color32(203,144,130, 0xff),
            new Color32(200,190,198, 0xff),
            new Color32(214,213,229, 0xff)
        };
    }

    public Color[] getColors()
    {
        Color[] c = new Color[1024 * 1024];
        float bucketInterval = 100f / buckets.Length;
        for (int x =0; x<xSize; x++){
            for (int y = 0; y < ySize; y++) {
                c[y * 1024 + x] = MyUtil.bucketize(heightMap[x,y], buckets, 0, 100);
            }
        }
        return c;
    }

    

    private float remap (float value, float a, float b, float c, float d)
    {
        value /= (b - a);
        value -= a;
        value *= (d - c);
        value += c;
        return value;
    }

}