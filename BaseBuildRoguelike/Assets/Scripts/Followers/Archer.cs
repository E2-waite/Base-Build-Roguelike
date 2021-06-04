using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Follower
{
    public float fireRange = 5f, shotCooldown = 2.5f, shotSpeed = 5, targetRange = 15f;
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



    public void Direct(Vector2 pos, GameObject obj)
    {
        canAttack = true;
        marker.transform.position = pos;
        if (obj != null)
        {
            Debug.Log("Target: " + obj.name);
            target = obj.GetComponent<Interaction>();

            marker.transform.position = obj.transform.position;


            if (target.type == Interaction.InteractionType.enemy)
            {
                state = State.attack;
            }
            else if (target.type == Interaction.InteractionType.building)
            {
                if (target.building.isConstructed)
                {
                    state = State.defend;
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
            target = newTarget.interaction;
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
