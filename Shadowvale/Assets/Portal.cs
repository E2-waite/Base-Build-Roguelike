using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Building
{
    

    void Activate()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            Grid.GetTile(tiles[i]).Corrupt(tiles[i]);
        }
    }
}
