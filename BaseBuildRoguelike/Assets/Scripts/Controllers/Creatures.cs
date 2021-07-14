using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Creatures
{
    public static List<Interaction> creatures = new List<Interaction>();
    public static int maxCreatures;
    public static void Add(Interaction creature)
    {
        creatures.Add(creature);
    }
    public static void Remove(Interaction creature)
    {
        creatures.Remove(creature);
    }
}
