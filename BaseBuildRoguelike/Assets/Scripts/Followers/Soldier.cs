using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Follower
{
    [Header("Soldier Settings")]
    public float hitSpeed = 1;

    private void Update()
    {
        if (state == State.move)
        {
            if (transform.position == marker.transform.position)
            {
                state = State.idle;
            }
            else
            {
                Move(marker.transform.position);
            }
        }
        else
        {
            if (target == null)
            {
                if (state == State.attack)
                {
                    if (FindTarget())
                    {
                        Debug.Log("Target Found");
                    }
                    else
                    {
                        state = State.idle;
                    }
                }
                else
                {
                    state = State.idle;
                }
            }
            else
            {
                if (Vector2.Distance(transform.position, target.transform.position) <= targetDist)
                {
                    if (state == State.attack && canAttack)
                    {
                        StartCoroutine(AttackRoutine());
                    }
                }
                else
                {
                    Move(target.transform.position);
                }
            }
        }
    }

    IEnumerator AttackRoutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(1 / hitSpeed);
        if (target != null && Vector2.Distance(transform.position, target.transform.position) <= targetDist)
        {
            Enemy enemy = target as Enemy;
            enemy.Hit(hitDamage, this);
        }
        canAttack = true;
    }
}