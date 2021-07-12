using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Priest : Follower
{
    public int healAmount = 5;
    public float healCooldown = 5;
    public bool canHeal = true, healing = false;
    void Update()
    {
        if (canHeal)
        {
            canHeal = false;
            StartCoroutine(HealRoutine());
        }
        if (!healing)
        {
            Swarm();
            Move();
        }
    }

    void Heal()
    {
        healing = false;
        anim.SetBool("Heal", false);
        if (squad != null)
        {
            int highestDamage = 0;
            Follower toHeal = null;
            foreach (Follower follower in squad.members)
            {
                int damage = follower.maxHealth - follower.health;
                if (damage > highestDamage)
                {
                    highestDamage = damage;
                    toHeal = follower;
                }
            }

            if (toHeal != null)
            {
                toHeal.Heal(healAmount);
            }
        }
        else
        {
            Heal(healAmount);
        }
    }

    IEnumerator HealRoutine()
    {
        healing = true;
        anim.SetBool("Heal", true);
        yield return new WaitForSeconds(healCooldown);
        canHeal = true;
    }
}
