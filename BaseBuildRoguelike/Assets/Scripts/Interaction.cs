using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{   
    public enum InteractionType
    {
        follower,
        resource,
        building,
        creature,
        enemy
    }
    public InteractionType type;

    [HideInInspector] public Follower follower;
    [HideInInspector] public Resource resource;
    [HideInInspector] public Building building;
    [HideInInspector] public Creature creature;
    [HideInInspector] public Enemy enemy;

    private void Start()
    {
        if (type == InteractionType.follower)
        {
            follower = GetComponent<Follower>();
        }
        else if (type == InteractionType.resource)
        {
            resource = GetComponent<Resource>();
        }
        else if (type == InteractionType.building)
        {
            building = GetComponent<Building>();
        }
        else if (type == InteractionType.creature)
        {
            creature = GetComponent<Creature>();
        }
        else if (type == InteractionType.enemy)
        {
            enemy = GetComponent<Enemy>();
        }
    }
}
