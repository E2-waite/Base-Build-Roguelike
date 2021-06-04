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
        home
    }
    [Header("Building Settings")]
    public Type type;
    public bool isConstructed = false;
    public bool selected = false;

    public int repair, maxRepair = 25;

    [HideInInspector] public Construct construct;
    [HideInInspector] public Interaction interaction;


    private void Start()
    {
        repair = maxRepair;
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
            GameController.Instance.inspector.Reload(this);
        }
    }

    public bool Hit(int damage)
    {
        repair -= damage;

        if (repair <= 0)
        {
            Destroy(gameObject);
            return true;
        }
        return false;
    }

}
