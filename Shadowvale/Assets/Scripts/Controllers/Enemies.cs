using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Enemies
{
    public static List<Enemy> enemies = new List<Enemy>();
    public static List<Squad> squads = new List<Squad>();
    public static void Add(Enemy enemy)
    {
        enemies.Add(enemy);
    }
    public static void Remove(Enemy enemy)
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

    public static void Reset()
    {
        enemies = new List<Enemy>();
        squads = new List<Squad>();
    }
}