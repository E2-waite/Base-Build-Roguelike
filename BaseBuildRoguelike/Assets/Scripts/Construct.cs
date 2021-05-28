using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construct : MonoBehaviour
{
    public Sprite contruction, constructed;
    public int woodCost, stoneCost, woodRemaining, stoneRemaining;
    Building building;
    SpriteRenderer rend;
    private void Start()
    {
        rend.sprite = contruction;
    }

    public void Setup(int wood, int stone)
    {
        building = GetComponent<Building>();
        rend = GetComponent<SpriteRenderer>();
        woodCost = wood;
        woodRemaining = wood;
        stoneCost = stone;
        stoneRemaining = stone;

        CheckComplete();
    }

    public void Build(Inventory inv)
    {
        if (woodRemaining > 0 && inv.wood > 0)
        {
            woodRemaining--;
            inv.wood--;
        }

        if (stoneRemaining > 0 && inv.stone > 0)
        {
            stoneRemaining--;
            inv.stone--;
        }

        CheckComplete();
    }

    void CheckComplete()
    {
        if (woodRemaining <= 0 && stoneRemaining <= 0)
        {
            building.Constructed();
            rend.sprite = constructed;
            Destroy(this);
        }
    }
}
