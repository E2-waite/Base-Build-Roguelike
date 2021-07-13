using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Follower
{
    public float fireRange = 5f, shotTime = 0.5f, shotSpeed = 10, shotCooldown = 2.5f;
    public GameObject arrowPrefab;
    bool firing = false;

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
                        // Debug.Log("Target Found");
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
                if (dist <= fireRange)
                {
                    // Moves away from target if not charging up shot, the target is an enemy and this archer is too close
                    //if (!attacking && dist < fireRange - 3f && target is Enemy)
                    //{
                    //    Move(transform.position + ((transform.position - target.transform.position).normalized));
                    //}
                    if (state == State.attack && canAttack)
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