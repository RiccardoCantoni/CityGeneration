using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph<T,Te>{

    public Dictionary<T, List<Edge<T,Te>>> edges;
    bool isDirected;

    public Graph(bool isDirected)
    {
        edges = new Dictionary<T, List<Edge<T, Te>>>();
        this.isDirected = isDirected;
    }

    public void addNode(T node)
    {
        if (!this.containsNode(node)) edges.Add(node, new List<Edge<T,Te>>());
    }

    public void removeNode(T node)
    {
        foreach (Edge<T,Te> edge in edges[node])
        {
            removeEdge(edge);
        }
        edges.Remove(node);
    }

    public void addEdge(T from, T to, Te data)
    {
        Edge<T, Te> e = new Edge<T, Te>(from, to, data);
        if (!this.containsNode(from)) this.addNode(from);
        if (!this.containsNode(to)) this.addNode(to);
        if (!this.containsEdge(e)) edges[e.from].Add(e);
        e = e.invert();
        if (!isDirected && !this.containsEdge(e)) edges[e.from].Add(e);
    }

    public void removeEdge(Edge<T,Te> e)
    {
        if (!this.containsNode(e.from)) throw new System.Exception("node not found: " + e.to.ToString());
        if (!this.containsNode(e.from)) throw new System.Exception("node not found: " + e.to.ToString());
        try
        {
            edges[e.from].Remove(e);
            if (!isDirected) edges[e.to].Remove(e.invert());
        }
        catch (System.Exception ex) { Debug.Log(ex.StackTrace); }
    }

    public List<Edge<T,Te>> outgoingEdges(T node)
    {
        return edges[node];
    }

    public List<T> successors (T node)
    {
        return edges[node].Select(e => e.to).ToList();
    }

    public int outDegree(T node)
    {
        return successors(node).Count;
    }

    public bool containsNode(T node)
    {
        return edges.ContainsKey(node);
    }

    public bool containsEdge(Edge<T,Te> e)
    {
        if (!this.containsNode(e.from)) return false;
        if (!this.containsNode(e.to)) return false;
        return edges[e.from].Contains(e);
    }

    public List<T> neighbourhood(T node, int size)
    {
        List<T> ls = new List<T>();
        List<T> visited = new List<T>();
        List<T> fringe = new List<T>();
        fringe.Add(node);
        List<T> succ;
        Dictionary<T, int> nodeDepth = new Dictionary<T, int>();
        nodeDepth.Add(node, 0);
        
        T cur;
        int depth;
        
        while (true)
        {
            if (fringe.Count == 0) break;
            cur = fringe[0];
            fringe.RemoveAt(0);
            if (visited.Contains(cur)) continue;
            visited.Add(cur);
            ls.Add(cur);
            depth = nodeDepth[cur];
            if (depth < size)
            {
                succ = successors(cur);
                succ = succ.Where(n => !visited.Contains(n)).ToList();
                fringe.AddRange(succ);
                foreach (T n in succ)
                {
                    if (nodeDepth.ContainsKey(n))
                    {
                        nodeDepth[n] = Mathf.Min(nodeDepth[n], depth + 1);
                    }
                    else
                    {
                        nodeDepth.Add(n, depth + 1);
                    }
                }
            }
            nodeDepth.Remove(cur);
        }
        ls.RemoveAt(0);
        return ls;
    }
}
