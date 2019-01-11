using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge<T,T1> {

    public T from, to;
    public T1 payload;

    public Edge(T from, T to, T1 payload)
    {
        this.from = from;
        this.to = to;
        this.payload = payload;
    }

    public Edge(T from, T to)
    {
        this.from = from;
        this.to = to;
    }

    public Edge<T,T1> invert()
    {
        return new Edge<T,T1>(this.to, this.from, this.payload);
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        if (!obj.GetType().Equals(typeof(Edge<T,T1>))) return false;
        Edge<T,T1> e = (Edge<T,T1>)obj;
        return (this.from.Equals(e.from) && this.to.Equals(e.to));
    }

    public override int GetHashCode()
    {
        int hash = 3;
        hash = (hash * 7) + this.from.GetHashCode();
        hash = (hash * 13) + this.to.GetHashCode(); ;
        return hash;
    }
}
