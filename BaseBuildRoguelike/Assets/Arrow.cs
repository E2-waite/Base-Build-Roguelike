using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public void Move(Interaction target, Follower follower, float shotSpeed, int hitDamage)
    {
        StartCoroutine(MoveTowardsTarget(target, follower, shotSpeed, hitDamage));
    }

    IEnumerator MoveTowardsTarget(Interaction target, Follower follower, float shotSpeed, int hitDamage)
    {
        if (target == null)
        {
            Destroy(gameObject);
        }
        else
        {
            if (transform.position == target.transform.position)
            {
                target.enemy.Hit(hitDamage, follower);
                Destroy(gameObject);
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, target.transform.position, shotSpeed * Time.deltaTime);
            }

            yield return null;
            StartCoroutine(MoveTowardsTarget(target, follower, shotSpeed, hitDamage));
        } 
    }
}
