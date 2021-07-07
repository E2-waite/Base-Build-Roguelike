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
        woodRemaining = woodCost;
        stoneRemaining = stoneCost;
        rend = GetComponent<SpriteRenderer>();
        rend.sprite = contruction;
        building = GetComponent<Building>();
        CheckComplete();
    }

    public void Build()
    {
        if (!building.isConstructed)
        {
            if (woodRemaining > 0 && GameController.Instance.resources[(int)Resource.Type.wood] > 0)
            {
                if (BuildingController.Instance.UseResource(Resource.Type.wood, 1))
                {
                    woodRemaining--;
                }
            }

            if (stoneRemaining > 0 && GameController.Instance.resources[(int)Resource.Type.stone] > 0)
            {

                if (BuildingController.Instance.UseResource(Resource.Type.stone, 1))
                {
                    stoneRemaining--;
                }
            }

            building.ReloadInspector();

            CheckComplete();
        }
    }

    void CheckComplete()
    {
        if (woodRemaining <= 0 && stoneRemaining <= 0)
        {
            rend.sprite = constructed;
            building.Constructed();
            Destroy(this);
        }
    }
}
