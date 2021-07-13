using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowBolt : Projectile
{
    public override void Hit(Interaction to, Interaction from, int damage)
    {
        if (to != null)
        {
            if (to is Follower)
            {
                (to as Follower).Hit(damage, from as Enemy);
            }
            to.AddEffect(new ShadowEffect(3, 1, to));
        }
        Destroy(gameObject);
    }
}
