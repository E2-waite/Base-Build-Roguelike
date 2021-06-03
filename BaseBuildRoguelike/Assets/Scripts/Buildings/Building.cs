using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public enum Type
    {
        house,
        storage,
        wall,
        main
    }
    [Header("Building Settings")]
    public Type type;
    public bool isConstructed = false;
    public bool selected = false;

    [HideInInspector] public Construct construct;
    [HideInInspector] public Interaction interaction;


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
            ResourceStorage storage = GetComponent<ResourceStorage>();
            storage.Setup();
        }
        else if (type == Type.wall)
        {
            Wall wall = GetComponent<Wall>();
            wall.Setup();
        }
        else if (type == Type.house)
        {
            FollowerController.Instance.AdjustMaxFollowers(5);
        }

        ReloadInspector();
    }

    public void ReloadInspector()
    {
        if (selected)
        {
            Inspector.Instance.Reload(this);
        }
    }
}
