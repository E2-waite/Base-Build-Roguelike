using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Follower
{
    enum State
    {
        idle = 0,
        move = 1,
        attack = 2,
        defend = 3
    }
    public float fireRange = 5f, shotTime = 0.5f, shotSpeed = 10, shotCooldown = 2.5f;
    public GameObject arrowPrefab;

    private void Update()
    {
        Swarm();
        if (state == (int)State.move)
        {
            if (transform.position == marker.transform.position)
            {
                state = (int)State.idle;
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
                if (state == (int)State.attack)
                {
                    if (Targetting.FindTarget(ref target, squad, ref targetSquad, transform.position, Enemies.enemies))
                    {
                        // Debug.Log("Target Found");
                    }
                    else
                    {
                        state = (int)State.move;
                    }
                }
                else
                {
                    state = (int)State.idle;
                }
            }
            else
            {
                float dist = Vector2.Distance(transform.position, target.transform.position);
                if (dist <= fireRange)
                {
                    // Moves away from target if not charging up shot, the target is an enemy and this archer is too close
                    //if (!attacking && dist < fireRange - 3f && target is Enemy)
                    //{
                    //    Move(transform.position + ((transform.position - target.transform.position).normalized));
                    //}
                    if (state == (int)State.attack && canAttack)
                    {
                        StartCoroutine(FireRoutine());
                    }
                }
                else
                {
                    Move();
                }
            }
        }
    }

    IEnumerator FireRoutine()
    {
        canAttack = false;
        attacking = true;
        yield return new WaitForSeconds(shotTime);
        attacking = false;
        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        arrow.GetComponent<Arrow>().Setup(target, this, shotSpeed, hitDamage);
        StartCoroutine(ShotCooldown());
    }

    IEnumerator ShotCooldown()
    {
        yield return new WaitForSeconds(shotCooldown);
        canAttack = true;
    }
}