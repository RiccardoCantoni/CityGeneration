using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class RoadNetworkGenerator : MonoBehaviour {

    public int segmentLimit;
    public float MIN_SEGMENT_LENGTH, MAX_SEGMENT_LENGTH;
    public float SNAP_RADIUS;
    public float CONTINUATION_DEVIATION_ANGLE;
    public float MIN_SNAP_ANGLE;
    public float FORK_CHANCE;
    public float MAX_INCLINATION;

    float[,] heightMap;

    // Use this for initialization
	public Graph<Vector2, Segment> generateNetwork (float[,] heightMap) {
         this.heightMap = heightMap;
         List<Vector2> seed = new List<Vector2>();
         seed.Add(new Vector2(0, 0));
         seed.Add(new Vector2(50, 50));
         Graph<Vector2, Segment> g = generate(seed);
         return g;
	}

    Graph<Vector2, Segment> G;
    List<Segment> aliveSegments;

    Graph<Vector2, Segment> generate(List<Vector2> seed) {
        int segmentCount = 0;
        G = new Graph<Vector2, Segment>(false);
        aliveSegments = new List<Segment>();
        Segment cur, continuation, fork;
        Vector2 v;
        foreach (Vector2 s in seed) {
            v = Random.insideUnitCircle * (MIN_SEGMENT_LENGTH+1);
            cur = new Segment(s, s + v);
            aliveSegments.Add(cur);
            G.addEdge(cur.from, cur.to, cur);
        }
        while (aliveSegments.Count > 0 && segmentCount < segmentLimit)
        {
            cur = aliveSegments[0];
            aliveSegments.Remove(cur);
            if (!isInMap(cur))
            {
                segmentCount++;
                continue;
            }
            continuation = proposeContinuation(cur);
            if (tryAdd(continuation)) segmentCount++;
            if (Randomiser.rollUnder(FORK_CHANCE)) { 
                fork = proposeFork(continuation, 90);
                if (tryAdd(fork)) segmentCount++;
            }
            if (Randomiser.rollUnder(FORK_CHANCE))
            {
                fork = proposeFork(continuation, -90);
                if (tryAdd(fork)) segmentCount++;
            }
        }
        return G;
	}

    Segment proposeContinuation(Segment current)
    {
        List<Segment> candidates = new List<Segment>();
        Vector2 v2 = (current.to - current.from).normalized * Random.Range(MIN_SEGMENT_LENGTH, MAX_SEGMENT_LENGTH);
        Vector3 v3;
        float angle = CONTINUATION_DEVIATION_ANGLE / 7f * 2f;
        for (int ii = -3; ii <= 3; ii++)
        {
            v3 = (Quaternion.AngleAxis(angle * ii, Vector3.up) * new Vector3(v2.x, 0, v2.y)) + new Vector3(current.to.x, 0, current.to.y);
            candidates.Add(new Segment(current.to, new Vector2(v3.x, v3.z)));
        }
        float hh = getHeight(current.to), minDelta = float.MaxValue, delta;
        Segment seg = candidates[0];
        foreach (Segment s in candidates)
        {
            delta = Mathf.Abs(hh - getHeight(s.to));
            if (delta < minDelta)
            {
                seg = s;
                minDelta = delta;
            }
        }
        return seg;
    }

    Segment proposeFork(Segment parent, float forkAngle)
    {
        Vector2 v2 = (parent.to - parent.from).normalized;
        Vector3 v3 = new Vector3(v2.x, 0, v2.y);
        float angle = Random.Range(CONTINUATION_DEVIATION_ANGLE / -2f, CONTINUATION_DEVIATION_ANGLE / 2f);
        v3 = Quaternion.AngleAxis(angle + forkAngle, Vector3.up) * v3;
        v3 = v3 * Random.Range(parent.length() * 0.6f, parent.length() * 1.1f);
        return new Segment(parent.from, parent.from + new Vector2(v3.x, v3.z));
    }

    bool doesSnap(Segment s, out Vector2 snapPoint, float snapRadiusFactor=1)
    {
        foreach (Vector2 vv in G.edges.Keys)
        {
            if (Vector2.Distance(s.to, vv) < SNAP_RADIUS*snapRadiusFactor)
            {
                snapPoint = vv;
                return true;
            }
        }
        snapPoint = Vector2.zero;
        return false;
    }

    bool doesIntersect(Segment s, out Segment other, out Vector2 intersection)
    {
        Segment i = null;
        Dictionary<Segment, Vector2> candidates = new Dictionary<Segment, Vector2>();
        foreach (KeyValuePair<Vector2, List<Edge<Vector2, Segment>>> kv in G.edges)
        {
            foreach (Edge<Vector2, Segment> e in kv.Value)
            {
                i = new Segment(e.from, e.to);
                if (Segment.intersect(s, i, out intersection))
                {
                    candidates.Add(i, intersection);
                }
            }
        }
        intersection = Vector2.zero;
        other = null;
        if (candidates.Count == 0) return false;
        float minDistance = float.MaxValue, d;
        foreach (KeyValuePair<Segment, Vector2> kv in candidates)
        {
            d = Vector2.Distance(s.from, kv.Value);
            if (d < minDistance)
            {
                other = kv.Key;
                minDistance = d;
                intersection = kv.Value;
            }
        }
        return true;
    }

    bool isValidSegment(Segment s)
    {
        float l = s.length();
        if (l < MIN_SEGMENT_LENGTH || l > MAX_SEGMENT_LENGTH) return false;
        float dh = Mathf.Abs(getHeight(s.from) - getHeight(s.to));
        if (inclination(s) > MAX_INCLINATION) return false;
        return true;
    }

    bool isValidAngle(Segment s1, Segment s2)
    {
        return Vector2.Angle(s1.to - s1.from, s2.to - s2.from) > MIN_SNAP_ANGLE;
    }

    bool tryAdd(Segment proposed)
    {
        if (!isValidSegment(proposed)) return false;
        Vector2 intersectionVector, snapVector, tempVector;
        Segment other, tempSegment;
        if (doesIntersect(proposed, out other, out intersectionVector))
        {
            intersectionVector =
                doesSnap(new Segment(proposed.from, intersectionVector), out snapVector, 0.5f) ?
                snapVector : intersectionVector;
            if (!isValidAngle(proposed, other))
                return false;
            if (doesIntersect(new Segment(proposed.from, intersectionVector), out tempSegment, out tempVector)) return false;
            if (!isValidAngle(new Segment(proposed.from, intersectionVector), new Segment(other.from, intersectionVector))) return false;
            if (!isValidAngle(new Segment(proposed.from, intersectionVector), new Segment(other.from, intersectionVector))) return false;
            G.removeEdge(new Edge<Vector2, Segment>(other.from, other.to));
            G.addEdge(other.from, intersectionVector, new Segment(other.from,intersectionVector));
            G.addEdge(intersectionVector, other.to, new Segment(other.to, intersectionVector));
            G.addEdge(proposed.from, intersectionVector, new Segment(proposed.from, intersectionVector));
            return true;
        }
        if (doesSnap(proposed, out snapVector))
        {
            foreach (Vector2 succ in G.successors(snapVector))
            {
                if (!isValidAngle(new Segment(proposed.from, snapVector), new Segment(snapVector, succ))) return false;
            }
            if (doesIntersect(new Segment(proposed.from, snapVector), out tempSegment, out tempVector)) return false;
            G.addEdge(proposed.from, snapVector, new Segment(proposed.from, snapVector));
            return true;
        }
        G.addEdge(proposed.from, proposed.to, proposed);
        aliveSegments.Add(proposed);
        return true;
    }

    public float getHeight(Vector2 v)
    {
        return heightMap[(int)v.x+512, (int)v.y+512];
    }

    public float inclination(Segment s)
    {
        float dh = Mathf.Abs(getHeight(s.from) - getHeight(s.to));
        return dh / s.length();
    }

    private bool isInMap(Segment s)
    {
        return s.to.x+512 >= 0 && s.to.x+512 < 1024 && s.from.x+512 >= 0 && s.to.x+512 < 1024;
    }

}
