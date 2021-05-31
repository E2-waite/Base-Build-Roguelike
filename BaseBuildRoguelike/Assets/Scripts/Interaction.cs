using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{   
    public enum Type
    {
        resource,
        building,
        creature,
        enemy
    }
    public Type type;

    [HideInInspector] public Resource resource;
    [HideInInspector] public Building building;
    [HideInInspector] public Creature creature;
    private void Start()
    {
        if (type == Type.resource)
        {
            resource = GetComponent<Resource>();
        }
        else if (type == Type.building)
        {
            building = GetComponent<Building>();
        }
        else if (type == Type.creature)
        {
            creature = GetComponent<Creature>();
        }
    }
}
