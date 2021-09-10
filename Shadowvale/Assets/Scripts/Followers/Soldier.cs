using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Combat

{
    [Header("Soldier Settings")]
    public float hitSpeed = 1;

    public override void Direct(Vector2 pos, Interaction obj)
    {
        base.Direct(pos, obj);

    }

    public override void Update()
    {
        base.Update();
    }

    public override bool Attack()
    {
        float dist = Vector2.Distance(transform.position, currentAction.target.Position());
        if (dist <= targetDist)
        {
            if (interactRoutine == null)
            {
                interactRoutine = StartCoroutine(AttackRoutine());
            }
            return true;
        }
        else if (dist <= chaseDist)
        {
            Move(currentAction.target.Position());
            return true;
        }
        return false;
    }
    IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(1 / hitSpeed);
        if (currentAction.target.interact != null && Vector2.Distance(transform.position, currentAction.target.Position()) <= targetDist)
        {
            Enemy enemy = currentAction.target.interact as Enemy;
            enemy.Hit(hitDamage, this);
        }
        interactRoutine = null;
    }
}