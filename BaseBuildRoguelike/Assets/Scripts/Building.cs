using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public bool isConstructed = false;

    [HideInInspector] public Construct construct;
    [HideInInspector] public ResourceStorage storage;
    [HideInInspector] public Interaction interaction;
    [HideInInspector] public Wall wall;

    public enum Type
    {
        house,
        storage,
        wall
    }
    public Type type;
    private void Start()
    {
        interaction = GetComponent<Interaction>();
        if (!isConstructed)
        {
            construct = GetComponent<Construct>();
        }
    }

    public void Constructed()
    {
        isConstructed = true;
        if (type == Type.storage)
        {
            storage = GetComponent<ResourceStorage>();
            if (storage.type == Resource.Type.wood)
            {
                BuildingController.Instance.woodPiles.Add(this);
            }
            else if (storage.type == Resource.Type.stone)
            {
                BuildingController.Instance.stonePiles.Add(this);
            }
            GameController.Instance.AdjustResources(storage.type, 0, storage.maxStorage);
        }
        else if (type == Type.wall)
        {
            wall = GetComponent<Wall>();
            wall.Setup();
        }
    }
}
