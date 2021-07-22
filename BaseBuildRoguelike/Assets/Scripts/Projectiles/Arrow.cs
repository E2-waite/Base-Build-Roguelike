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
            if (enemy.Hit(damage, from))
            {
                Destroy(gameObject);
            }
        }
        Destroy(gameObject);
    }
}
