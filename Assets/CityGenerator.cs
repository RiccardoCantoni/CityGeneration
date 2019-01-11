using UnityEngine;
using System.Collections.Generic;

public class CityGenerator : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        //Random.InitState(123);
        float[,] heightmap = new float[1024, 1024];
        for (int x=0; x<1024; x++)
        {
            for (int y=0; y<1024; y++)
            {
                heightmap[x, y] = getHeight(x, y);
            }
        }
        RoadNetworkGenerator rng = GetComponent<RoadNetworkGenerator>();
        Graph<Vector2, Segment> roadGraph = rng.generateNetwork(heightmap);
        roadGraph.addNode(new Vector2(511, 511));
        Dictionary<Vector2, Vector3> pos = new Dictionary<Vector2, Vector3>();
        foreach (Vector2 v in roadGraph.edges.Keys)
        {
            pos.Add(v, new Vector3(v.x, heightmap[(int)v.x+512,(int)v.y + 512], v.y));
        }
        transform.GetChild(0).GetComponent<RoadPGraph>().heightmap = heightmap;
        transform.GetChild(0).GetComponent<RoadPGraph>().drawGraph(roadGraph, pos);
        TestRenderer renderer = new TestRenderer(heightmap, 1024, 1024);
        transform.GetChild(1).GetComponent<PhysicalMap>().init(1024, 1024);
        transform.GetChild(1).GetComponent<PhysicalMap>().draw(renderer);
    }

    public float getHeight(int x, int y)
    {
        float scale = 0.002f;
        float n = 0;
        n += Mathf.PerlinNoise(x * scale, y * scale) * 4f;
        n += Mathf.PerlinNoise(x * scale * 2, y * scale * 2)*2;
        n += Mathf.PerlinNoise(x * scale * 4, y * scale * 4);
        n /= 7f;
        n = Mathf.Min(1, n);
        n = Mathf.Max(0, n);
        return n*100;
    }
}
