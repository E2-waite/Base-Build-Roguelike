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
}