using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construct : MonoBehaviour
{
    public Sprite contruction, constructed;
    public int woodCost, stoneCost, woodRemaining, stoneRemaining;
    Structure building;
    SpriteRenderer rend;
    private void Start()
    {
        rend.sprite = contruction;
    }

    public void Setup(int wood, int stone)
    {
        building = GetComponent<Structure>();
        rend = GetComponent<SpriteRenderer>();
        woodCost = wood;
        woodRemaining = wood;
        stoneCost = stone;
        stoneRemaining = stone;
        if (woodRemaining <= 0 && stoneRemaining <= 0)
        {
            building.isConstructed = true;
            rend.sprite = constructed;
            Destroy(this);
        }
    }

    void Build(int wood, int stone)
    {
        woodRemaining -= wood;
        stoneRemaining -= stone;

        if (woodRemaining <= 0 && stoneRemaining <= 0)
        {
            building.isConstructed = true;
            rend.sprite = constructed;
            Destroy(this);
        }
    }
}
