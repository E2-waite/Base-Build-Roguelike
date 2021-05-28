using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int wood = 0, stone = 0, food = 0, capacity = 10; 
    
    public bool AtCapacity()
    {
        if (wood + stone + food >= capacity)
        {
            return true;
        }
        return false;
    }
}
