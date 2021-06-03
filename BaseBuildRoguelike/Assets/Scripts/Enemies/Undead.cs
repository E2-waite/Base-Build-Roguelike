using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Undead : Enemy
{
    public enum UndeadType
    { 
        worker,
        soldier
    }

    [Header("Undead Settings")]
    public UndeadType undeadType;


    private void Update()
    {
        if (target == null)
        {
            target = GameController.Instance.homeBuilding;
        }
        else
        {
            if (Vector2.Distance(transform.position, target.transform.position) <= targetDist)
            {
                // Attack

            }
            else
            {
                Move(target.transform.position);

            }
        }
    }


}
