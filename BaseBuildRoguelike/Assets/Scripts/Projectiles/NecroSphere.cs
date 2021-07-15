using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecroSphere : Projectile
{
    public override void Hit(Interaction to, Interaction from, int damage)
    {
        if (to != null && to is Corpse)
        {
            (to as Corpse).Revive(from as Necromancer);
        }
        Destroy(gameObject);
    }
}
