using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Creatures
{
    public static List<Interaction> creatures = new List<Interaction>();
    public static int maxCreatures;
    public static void Add(Creature creature)
    {
        creatures.Add(creature);
    }
    public static void Remove(Creature creature)
    {
        creatures.Remove(creature);
    }

    public static void Reset()
    {
        creatures = new List<Interaction>();
    }
}
