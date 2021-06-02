using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Inspector : MonoSingleton<Inspector>
{
    public enum Type
    {
        none,
        resources,
        construction
    }
    public Type currentType;
    public Text title;

    public ResourceDetails resources;
    public ConstructionDetails construction;

    public void Reload(Building building)
    {
        title.text = building.name;

        Type thisType = Type.none;


        if (building.isConstructed)
        {
            if (building.type == Building.Type.storage)
            {
                thisType = Type.resources;
            }
        }
        else
        {
            thisType = Type.construction;
        }

        // Only swaps activated details gameobject if not the same type
        if (thisType != currentType)
        {
            SwapDetails(thisType);
        }

        ReloadDetails(building);
    }



    void ReloadDetails(Building building)
    {
        if (currentType == Type.resources)
        {
            ResourceStorage storage = (ResourceStorage)building;
            resources.Reload(storage);
        }
        else if (currentType == Type.construction)
        {
            construction.Reload(building);
        }
    }

    void SwapDetails(Type type)
    {
        if (type == Type.resources)
        {
            resources.gameObject.SetActive(true);
            construction.gameObject.SetActive(false);
        }
        else if (type == Type.construction)
        {
            resources.gameObject.SetActive(false);
            construction.gameObject.SetActive(true);
        }
        else
        {
            resources.gameObject.SetActive(false);
            construction.gameObject.SetActive(false);
        }
        currentType = type;
    }
}
