using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomiser {

    public static T[] shuffle<T>(T[] array){
        int r;
        T temp;
        System.Random rnd = new System.Random();
        for (int i=0; i<array.Length; i++)
        {
            r = rnd.Next(i, array.Length - 1);
            if (r == i) continue;
            temp = array[i];
            array[i] = array[r];
            array[r] = temp;
        }
        return array;
    }

    public static T randomElement<T>(List<T> ls)
    {
        return ls[Random.Range(0, ls.Count - 1)];
    }

    public static T randomElement<T>(T[] ls)
    {
        return ls[Random.Range(0, ls.Length - 1)];
    }
    
    public static bool rollUnder(float f)
    {
        return Random.value < f;
    }
}
