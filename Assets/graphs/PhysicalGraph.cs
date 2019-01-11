using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalGraph<T,Te> : MonoBehaviour{

    public GameObject node;
    public GameObject edge;
    public GameObject parent;

    public void drawGraph(Graph<T,Te> g, Dictionary<T,Vector3> pos)
    {
        drawNodes(g,pos);
        drawEdges(g, pos);
    }

    protected void drawNodes(Graph<T,Te> g, Dictionary<T, Vector3> pos)
    {
        foreach (T node in g.edges.Keys)
        {
            instantiateNode(node, pos);
        }
    }

    protected void drawEdges(Graph<T,Te> g, Dictionary<T, Vector3> pos)
    {
        foreach (KeyValuePair<T, List<Edge<T,Te>>> kv in g.edges)
        {
            foreach (Edge<T,Te> e in kv.Value)
            {
                instantiateEdge(e, pos);
            }
        }
    }

    protected virtual void instantiateEdge(Edge<T,Te> e, Dictionary<T, Vector3> pos)
    {
        Debug.Log("to be implemented in subclass");
        throw new System.NotImplementedException();
    }

    protected virtual void instantiateNode(T n, Dictionary<T, Vector3> pos)
    {
        Debug.Log("to be implemented in subclass");
        throw new System.NotImplementedException();
    }




}
