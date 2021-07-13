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
        if (!healing)
        {
            Swarm();
            Move();
        }
    }

    public override void Setup()
    {
        StartCoroutine(HealthCheck());
    }

    Follower followerTarget = null;
    IEnumerator HealthCheck()
    {
        bool heal = false;
        followerTarget = null;
        while (!heal)
        {
            yield return new WaitForSeconds(1);
            if (squad != null)
            {
                int highestDamage = 0;
                foreach (Follower follower in squad.members)
                {
                    int damage = follower.maxHealth - follower.health;
                    if (damage > highestDamage)
                    {
                        highestDamage = damage;
                        followerTarget = follower;
                    }
                }

                if (followerTarget != null)
                {
                    heal = true;
                }
            }
            else
            {
                if (health < maxHealth)
                {
                    followerTarget = this;
                    heal = true;
                }
            }
        }
        healing = true;
        anim.SetBool("Heal", true);
    }


    void Heal()
    {
        anim.SetBool("Heal", false);
        if (followerTarget != null)
        {
            followerTarget.AddEffect(new HealEffect(1, 0.5f, followerTarget));
        }
        StartCoroutine(HealCooldown());
    }

    IEnumerator HealCooldown()
    {
        yield return new WaitForSeconds(0.1f);
        healing = false;
        yield return new WaitForSeconds(healCooldown);
        StartCoroutine(HealthCheck());
    }

}
