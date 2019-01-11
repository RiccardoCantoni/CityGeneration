using UnityEngine;
using UnityEditor;

public class MyUtil 
{
    public static float remap(float value, float a, float b, float c, float d)
    {
        value /= (b - a);
        value -= a;
        value *= (d - c);
        value += c;
        return value;
    }

    public static T bucketize<T>(float value, T[] buckets, float min, float max)
    {
        int v = (int)remap(value, min, max, 0, buckets.Length - 1);
        v = Mathf.Min(buckets.Length - 1, v);
        v = Mathf.Max(0, v);
        return buckets[v];
    }
}