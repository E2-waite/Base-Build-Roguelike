using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Follower
{
    [Header("Soldier Settings")]
    public float hitSpeed = 1;

    private void Update()
    {
        Swarm();
        if (state == State.move)
        {
            if (transform.position == marker.transform.position)
            {
                state = State.idle;
            }
            else
            {
                Move();
            }
        }
        else
        {
            if (target == null)
            {
                if (state == State.attack)
                {
                    if (Targetting.FindTarget(ref target, squad, ref targetSquad, transform.position, EnemyController.Instance.enemies))
                    {
                        Debug.Log("Target Found");
                    }
                    else
                    {
                        state = State.move;
                    }
                }
                else
                {
                    state = State.idle;
                }
            }
            else
            {
                float dist = Vector2.Distance(transform.position, target.transform.position);
                if (dist <= targetDist)
                {
                    if (state == State.attack && canAttack)
                    {
                        StartCoroutine(AttackRoutine());
                    }
                }
                else if (dist <= chaseDist)
                {
                    Move(target.transform.position);
                }
                else
                {
                    Move();
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