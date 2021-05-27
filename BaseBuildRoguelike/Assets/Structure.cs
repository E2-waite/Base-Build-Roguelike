using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public bool isConstructed = false;
    public ResourceStorage storage;
    public enum Type
    {
        house,
        storage
    }
    public Type type;
    private void Start()
    {
        if (type == Type.storage)
        {
            storage = GetComponent<ResourceStorage>();
        }
    }
}
