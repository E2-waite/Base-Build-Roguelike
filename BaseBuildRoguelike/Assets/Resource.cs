using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public int val = 10;
    public enum Type
    {
        wood,
        stone,
        food
    }
    public Type type;

    public bool Gather(Inventory inv)
    {
        if (type == Type.wood)
        {
            inv.wood++;
        }
        if (type == Type.stone)
        {
            inv.stone++;
        }
        if (type == Type.food)
        {
            inv.food++;
        }

        val--;
        if (val <= 0)
        {
            Destroy(gameObject);
        }
        return true;
    }
}
