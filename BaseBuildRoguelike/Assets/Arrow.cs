using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    Interaction target;
    Follower follower;
    float shotSpeed;
    int hitDamage;
    bool move = false;
    public void Move(Interaction _target, Follower _follower, float _shotSpeed, int _hitDamage)
    {
        target = _target;
        follower = _follower;
        shotSpeed = _shotSpeed;
        hitDamage = _hitDamage;
        move = true;
    }

    private void Update()
    {
        if (move & target != null)
        {
            float dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.transform.position, shotSpeed * Time.deltaTime);
            }
            else
            {
                target.enemy.Hit(hitDamage, follower);
                Destroy(gameObject);
            }
        }
        else if (target == null)
        {
            Destroy(gameObject);
        }
    }
}
