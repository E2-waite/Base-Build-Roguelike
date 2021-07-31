using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resources : MonoBehaviour
{
    public const int NUM = 3;
    public static int[] resources = new int[NUM];
    public static int[] maxResources = new int[NUM];

    public static List<Interaction> trees = new List<Interaction>(), stones = new List<Interaction>(), allResources = new List<Interaction>();

    public static void Adjust(Resource.Type type, int val, int maxVal)
    {
        Debug.Log("ADJUSTING");
        int pos = (int)type;
        resources[pos] += val;
        maxResources[pos] += maxVal;

        HUD.Instance.UpdateResources(resources, maxResources);
    }

    public static int Value(Resource.Type type)
    {
        return resources[(int)type];
    }

    public static void Reset()
    {
        resources = new int[NUM];
        resources = new int[NUM];
    }
}
