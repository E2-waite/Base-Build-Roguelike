using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Follower
{
    public enum State
    {
        idle,
        move,
        attack,
        defend
    }

    [Header("Soldier Settings")]
    public State state = State.idle;
    public Squad squad;
    public Interaction target;
    public float hitSpeed = 1;
    public bool canAttack = true;


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
                    // Find target
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

    public void TargetEnemy(Enemy enemy)
    {
        canAttack = true;
        target = enemy.interaction;
        state = State.attack;
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

    IEnumerator AttackRoutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(1 / hitSpeed);
        if (target != null && Vector2.Distance(transform.position, target.transform.position) <= targetDist)
        {
            target.enemy.Hit(hitDamage, this);
        }
        canAttack = true;
    }
}
