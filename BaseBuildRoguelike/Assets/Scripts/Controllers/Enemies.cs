using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Enemies
{
    public static List<Interaction> enemies = new List<Interaction>();

    public static void Add(Interaction enemy)
    {
        enemies.Add(enemy);
    }
    public static void Remove(Interaction enemy)
    {
        enemies.Remove(enemy);
    }

    public static Interaction Closest(Vector3 pos)
    {
        Interaction closest = null;
        float closestDist = 9999;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] != null)
            {
                float dist = Vector3.Distance(pos, enemies[i].transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = enemies[i];
                }
            }
        }
        return closest;
    }
}