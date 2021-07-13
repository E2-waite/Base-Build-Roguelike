using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{
    public override void Hit(Interaction to, Interaction from, int damage)
    {
        if (to is Enemy)
        {
            Enemy enemy = to as Enemy;
            enemy.Hit(damage, from as Follower);
        }
        Destroy(gameObject);
    }
}
