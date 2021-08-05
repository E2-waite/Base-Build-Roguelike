using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : Interaction
{

    [System.NonSerialized] public int type = -1;
    [Header("Building Settings")]
    public bool isConstructed = false;
    public bool selected = false;

    public int repair, maxRepair = 25;
    [HideInInspector] public Construct construct;
    protected SpriteRenderer rend;
    public Vector2Int[] tiles;
    public BuildingData buildingData = null;
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

    public void Centre()
    {
        Vector2 pos = tiles[0];

        // Center building if larger than a single tile
        if (tiles.Length > 1)
        {
            Vector2 centre = new Vector2();
            for (int i = 0; i < tiles.Length; i++)
            {
                centre += tiles[i];
            }
            pos = centre / tiles.Length;
        }

        transform.position = pos;
    }


    public bool Hit(int damage)
    {
        repair -= damage;
        ReloadInspector();
        StartCoroutine(HitRoutine());
        if (repair <= 0)
        {
            Remove();
            Destroy(gameObject);
            DestroyThis();
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

    public bool Repair()
    {
        if (repair < maxRepair)
        {
            repair++;
            ReloadInspector();
            return false;
        }
        return true;
    }

    public virtual bool Save(BuildingData data)
    {
        data.type = type;
        data.health = repair;
        data.tiles = tiles;
        if (construct == null)
        {
            return true;
        }
        data.constructed = false;
        data.resourceCost = construct.cost;
        data.resourceRemaining = construct.remaining;
        // If still under construction (return false) then do not load inherrited class.
        return false;
    }

    public virtual void Load (BuildingData data)
    {
        type = data.type;
        tiles = data.tiles;
        for (int j = 0; j < tiles.Length; j++)
        {
            Grid.tiles[tiles[j].x, tiles[j].y].structure = this;
        }
        Centre();
        Buildings.Add(this);
        buildingData = data;

        if (!(this is HomeBase) && !data.constructed)
        {
            construct = GetComponent<Construct>();
            construct.CheckComplete(data);
        }
    }


    public virtual void LoadInstance()
    {

    }

    public virtual void DestroyThis()
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
