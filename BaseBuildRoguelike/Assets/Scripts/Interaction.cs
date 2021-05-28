using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{   
    public enum Type
    {
        resource,
        building
    }
    public Type type;

    [HideInInspector] public Resource resource;

    [HideInInspector] public Building building;
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
    }

    public void GatherResource(Inventory inv)
    {
        resource.Gather(inv);
    }


}
