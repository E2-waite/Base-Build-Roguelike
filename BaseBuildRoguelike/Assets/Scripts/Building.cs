using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public bool isConstructed = false;

    [HideInInspector] public Construct construct;
    [HideInInspector] public ResourceStorage storage;
    [HideInInspector] public Interaction interaction;

    public enum Type
    {
        house,
        storage
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
        storage = GetComponent<ResourceStorage>();
        if (type == Type.storage)
        {
            if (storage.type == Resource.Type.wood)
            {
                BuildingController.Instance.woodPiles.Add(this);
            }
            else if (storage.type == Resource.Type.stone)
            {
                BuildingController.Instance.stonePiles.Add(this);
            }
        }
    }
}
