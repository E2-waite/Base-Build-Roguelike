using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Follower
{
    [Header("Soldier Settings")]
    public float hitSpeed = 1;
    enum State
    {
        idle = 0,
        move = 1,
        attack = 2,
        defend = 3
    }

    private void Update()
    {
        Swarm();
        TickEffects();
        if (currentAction.state == (int)State.move)
        {
            if (transform.position == marker.transform.position)
            {
                Idle();
            }
            else
            {
                Move();
            }
        }
        else
        {
            if (currentAction.target.interact == null)
            {
                if (currentAction.state == (int)State.attack)
                {
                    // Find target
                    //if (Targetting.FindTarget(ref target, squad, transform.position, Enemies.enemies))
                    //{
                    //    Debug.Log("Target Found");
                    //}
                    //else
                    //{

                    //}
                    MoveTo(marker.transform.position);
                }
                else
                {
                    Idle();
                }
            }
            else
            {
                float dist = Vector2.Distance(transform.position, currentAction.target.Position());
                if (dist <= targetDist)
                {
                    if (currentAction.state == (int)State.attack && interactRoutine == null)
                    {
                        interactRoutine = StartCoroutine(AttackRoutine());
                    }
                }
                else if (dist <= chaseDist)
                {
                    Move(currentAction.target.Position());
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
        yield return new WaitForSeconds(1 / hitSpeed);
        if (currentAction.target.interact != null && Vector2.Distance(transform.position, currentAction.target.Position()) <= targetDist)
        {
            Enemy enemy = currentAction.target.interact as Enemy;
            enemy.Hit(hitDamage, this);
        }
        interactRoutine = null;
    }
}