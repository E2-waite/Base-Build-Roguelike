using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Follower
{
    public float fireRange = 5f, shotCooldown = 2.5f, shotSpeed = 10, targetRange = 15f;
    public bool canAttack = true;
    public GameObject arrowPrefab;
    public enum State
    {
        idle,
        move,
        attack,
        defend
    }
    public State state = State.idle;

    Interaction target;


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
                if (Vector2.Distance(transform.position, target.transform.position) <= fireRange)
                {
                    if (state == State.attack && canAttack)
                    {
                        StartCoroutine(FireRoutine());
                    }
                }
                else
                {
                    Move(target.transform.position);
                }
            }
        }
    }



    public override void Direct(Vector2 pos, Interaction obj)
    {
        canAttack = true;
        marker.transform.position = pos;
        if (obj != null)
        {
            Debug.Log("Target: " + obj.name);
            target = obj;

            marker.transform.position = obj.transform.position;


            if (target is Enemy)
            {
                state = State.attack;
            }
            else if (target is Building)
            {
                if ((target as Building).isConstructed)
                {
                    state = State.defend;
                }
            }
            else if (target is Follower)
            {
                Follower follower = target as Follower;
                if (follower is Soldier || follower is Archer)
                {
                    JoinSquad(follower);
                }
            }
        }
        else
        {
            target = null;
            state = State.move;
        }
    }

    bool FindTarget()
    {
        Enemy newTarget = null;
        float closestDist = 1000;
        foreach (Enemy enemy in EnemyController.Instance.enemies)
        {
            if (enemy != null)
            {
                float dist = Vector3.Distance(transform.position, enemy.transform.position);

                if (dist <= targetRange && dist < closestDist)
                {
                    closestDist = dist;
                    newTarget = enemy;
                }
            }
        }

        if (newTarget != null)
        {
            target = newTarget;
            return true;
        }
        return false;
    }

    IEnumerator FireRoutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(shotCooldown);
        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        arrow.GetComponent<Arrow>().Move(target, this, shotSpeed, hitDamage);
        canAttack = true;
    }
}
