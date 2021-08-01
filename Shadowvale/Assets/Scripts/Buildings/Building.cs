using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : Interaction
{
    [Header("Building Settings")]
    public int type = 99;
    public bool isConstructed = false;
    public bool selected = false;

    public int repair, maxRepair = 25;
    [HideInInspector] public Construct construct;
    protected SpriteRenderer rend;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        repair = maxRepair;
        construct = GetComponent<Construct>();
    }

    public void Constructed()
    {
        isConstructed = true;
        Setup();
        ReloadInspector();
        Pathfinding.UpdateNodeGrid();
    }

    public virtual void Setup()
    {

    }

    public void ReloadInspector()
    {
        if (Buildings.selected == this)
        {
            Inspector.Enable(this);
        }
    }

    public bool Hit(int damage)
    {
        repair -= damage;
        StartCoroutine(HitRoutine());
        if (repair <= 0)
        {
            Remove();
            Destroy(gameObject);
            Destroy();
            return true;
        }
        return false;
    }

    IEnumerator HitRoutine()
    {
        // React to hit after delay
        rend.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.color = Color.white;
    }

    public virtual void Destroy()
    {

    }

    void Remove()
    {
        Buildings.buildings.Remove(this);
        Grid.GetTile(new Vector2Int((int)transform.position.x, (int)transform.position.y)).structure = null;
        if (this is HomeBase)
        {
            GameController.Instance.GameOver();
        }
        Pathfinding.UpdateNodeGrid();
    }
}
