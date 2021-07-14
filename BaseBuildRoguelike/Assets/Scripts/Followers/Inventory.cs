using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int[] resources = new int[Resources.NUM];
    public int capacity = 10;
    public bool AtCapacity()
    {
        int total = 0;
        for (int i = 0; i < Resources.NUM; i++)
        {
            total += resources[i];
        }

        if (total >= capacity)
        {
            return true;
        }
        return false;
    }
}
