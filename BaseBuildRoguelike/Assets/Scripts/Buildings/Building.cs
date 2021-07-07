using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : Interaction
{
    [Header("Building Settings")]
    public bool isConstructed = false;
    public bool selected = false;

    public int repair, maxRepair = 25;

    [HideInInspector] public Construct construct;
    protected SpriteRenderer rend;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        repair = maxRepair;
        if (!isConstructed)
        {
            construct = GetComponent<Construct>();
        }
    }

    public void Constructed()
    {
        isConstructed = true;
        Setup();
        ReloadInspector();
    }

    public virtual void Setup()
    {

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
