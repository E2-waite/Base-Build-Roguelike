using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construct : MonoBehaviour
{
    public Sprite contruction, constructed;
    #if UNITY_EDITOR
    [EnumNamedArray(typeof(Resource.Type))]
    #endif
    public int[] cost = new int[Resources.NUM], remaining = new int[Resources.NUM];
    Building building;
    SpriteRenderer rend;

    private void Start()
    {
        for (int i = 0; i < Resources.NUM; i++)
        {
            remaining[i] = cost[i];
        }
        rend = GetComponent<SpriteRenderer>();
        rend.sprite = contruction;
        building = GetComponent<Building>();
        CheckComplete();
    }

    public void Build()
    {
        if (!building.isConstructed)
        {
            for (int i = 0; i < Resources.NUM; i++)
            {
                if (remaining[i] > 0 && Resources.resources[i] > 0 && Buildings.UseResource((Resource.Type)i, 1))
                {
                    remaining[i]--;
                }
            }

            building.ReloadInspector();

            CheckComplete();
        }
    }

    public void CheckComplete()
    {
        for (int i = 0; i < Resources.NUM; i++)
        {
            if (remaining[i] > 0)
            {
                return;
            }
        }

        rend.sprite = constructed;
        building.Constructed();
        Destroy(this);
    }
}
