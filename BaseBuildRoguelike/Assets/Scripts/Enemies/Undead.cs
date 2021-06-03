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
    public float hitSpeed = 1;
    public bool canAttack = true;

    private void Update()
    {
        if (target == null)
        {
            target = GameController.Instance.homeBuilding;
        }
        else
        {
            if (canAttack && Vector2.Distance(transform.position, target.transform.position) <= targetDist)
            {
                // Attack
                StartCoroutine(AttackRoutine());

            }
            else
            {
                Move(target.transform.position);

            }
        }
    }

    IEnumerator AttackRoutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(1 / hitSpeed);
        if (target != null)
        {
            if (target.type == Interaction.InteractionType.follower && Vector2.Distance(transform.position, target.transform.position) <= targetDist)
            {
                target.follower.Hit(hitDamage, this);
            }
            else if (target.type == Interaction.InteractionType.building)
            {
                target.building.Hit(hitDamage);
            }
        }
        canAttack = true;
    }
}
