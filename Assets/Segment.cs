using UnityEngine;
using UnityEditor;

public class Segment{

    public Vector2 from;
    public Vector2 to;

    public Segment(Vector2 from, Vector2 to)
    {
        this.from = from;
        this.to = to;
    }

    public override int GetHashCode()
    {
        return 3 + 7 * from.GetHashCode() + 13 * to.GetHashCode();
    }

    public override string ToString()
    {
        return "s:" + from.ToString() + "," + to.ToString();
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        if (!(obj is Segment)) return false;
        Segment e = (Segment)obj;
        return (from.Equals(e.from) && to.Equals(e.to));
    }

    public float length()
    {
        return Vector2.Distance(from, to);
    }

    private static bool ccw(Vector2 A, Vector2 B, Vector2 C)
    {
        return (C.y - A.y) * (B.x - A.x) > (B.y - A.y) * (C.x - A.x);
    }

    public static bool intersect(Segment s1, Segment s2, out Vector2 intersection)
    {
        if (s1.from.Equals(s2.from) || s1.from.Equals(s2.to) || s1.to.Equals(s2.from) || s1.to.Equals(s2.to))
        {
            intersection = Vector2.zero;
            return false;
        }
        Vector2 r = s1.to - s1.from;
        Vector2 s = s2.to - s2.from;
        var rs = cross(r, s);
        if (rs == 0)
        {
            intersection = Vector2.zero;
            return false;
        }
        float t = cross(s2.from - s1.from, s) / rs;
        float u = cross(s2.from - s1.from, r) / rs;
        if (u > 0 & u < 1 & t > 0 & t < 1)
        {
            intersection = s1.from + t*r;
            return true;
        } else
        {
            intersection = Vector2.zero;
            return false;
        }
    }

    public static bool intersect(Segment s1, Segment s2)
    {
        Vector2 r = s1.to - s1.from;
        Vector2 s = s2.to - s2.from;
        var rs = cross(r, s);
        if (rs == 0)  return false;
        float t = cross(s2.from - s1.from, s) / rs;
        float u = cross(s2.from - s1.from, r) / rs;
        return (u > 0 & u < 1 & t > 0 & t < 1);
    }

    static float cross(Vector2 v, Vector2 w)
    {
        return (v.x * w.y - v.y * w.x);
    }




}
 