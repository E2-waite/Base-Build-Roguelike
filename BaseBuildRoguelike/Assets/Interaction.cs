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

    Resource resource;
    public Structure building;
    private void Start()
    {
        if (type == Type.resource)
        {
            resource = GetComponent<Resource>();
        }
        else if (type == Type.building)
        {
            building = GetComponent<Structure>();
        }
    }

    public void GatherResource(Inventory inv)
    {
        resource.Gather(inv);
    }


}
