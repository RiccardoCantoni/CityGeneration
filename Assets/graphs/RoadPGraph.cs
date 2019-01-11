using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RoadPGraph : PhysicalGraph<Vector2, Segment>
{
    public float[,] heightmap;
    Color[] edgecolors;

    public void Awake()
    {
        edgecolors = new Color[]
            {
            Color.green,
            Color.Lerp(Color.green,Color.red, 0.1f),
            Color.Lerp(Color.green,Color.red, 0.2f),
            Color.Lerp(Color.green,Color.red, 0.3f),
            Color.Lerp(Color.green,Color.red, 0.4f),
            Color.Lerp(Color.green,Color.red, 0.5f),
            Color.Lerp(Color.green,Color.red, 0.6f),
            Color.Lerp(Color.green,Color.red, 0.7f),
            Color.Lerp(Color.green,Color.red, 0.8f),
            Color.Lerp(Color.green,Color.red, 0.9f),
            Color.red
            };
}



    protected override void instantiateNode(Vector2 n, Dictionary<Vector2, Vector3> pos)
    {
        GameObject go = Instantiate(node, pos[n], Quaternion.identity, parent.transform);
        go.transform.LookAt(Vector3.up + go.transform.position);
    }

    protected override void instantiateEdge(Edge<Vector2, Segment> e, Dictionary<Vector2, Vector3> pos)
    {
        GameObject go = Instantiate(edge, parent.transform);
        LineRenderer lr = go.GetComponent<LineRenderer>();
        lr.SetPositions(new Vector3[]{
            pos[e.from],
            pos[e.to]});
        //Color c = MyUtil.bucketize(inclination(e.payload), edgecolors, 0.1f, 0.2f);
        //lr.startColor = c;
        //lr.endColor = c;
        go.name = "edge" + inclination(e.payload);
    }

    public float inclination(Segment s)
    {
        float dh = Mathf.Abs(heightmap[(int)s.from.x + 512, (int)s.from.y + 512] - heightmap[(int)s.to.x + 512, (int)s.to.y + 512]);
        return dh / s.length();
    }




}