using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Follower
{
    public float fireRange = 5f, shotCooldown = 2.5f, shotSpeed = 10;
    public GameObject arrowPrefab;

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
                    if (Targetting.FindTarget(ref target, squad, ref targetSquad, transform.position, EnemyController.Instance.enemies))
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

    IEnumerator FireRoutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(shotCooldown);
        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        arrow.GetComponent<Arrow>().Move(target, this, shotSpeed, hitDamage);
        canAttack = true;
    }
}