using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Priest : Follower
{
    enum State
    {
        idle = 0,
        move = 1,
        heal = 2,
    }
    public int healAmount = 5;
    public float healRange = 5;
    public bool canHeal = true, healing = false;
    Cooldown healCooldown = new Cooldown(5);
    Coroutine healthCheck = null;
    Follower healTarget = null;
    void Update()
    {
        TickEffects();
        if (healCooldown.Tick() && healthCheck == null)
        {
            // If cooled down and not currently checking health
            healthCheck = StartCoroutine(HealthCheck());
        }

        //if (state == (int)State.move)
        //{
        //    if (transform.position == marker.transform.position)
        //    {
        //        state = (int)State.idle;
        //    }
        //    else
        //    {
        //        Move();
        //    }
        //}

        if (currentAction.state != (int)State.heal)
        {
            Swarm();
            Move();
        }
    }

    public override void Setup()
    {
        //StartCoroutine(HealthCheck());
    }


    IEnumerator HealthCheck()
    {
        bool heal = false;
        healTarget = null;
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
                        healTarget = follower;
                    }
                }

                if (healTarget != null)
                {
                    heal = true;
                }
            }
            else
            {
                if (health < maxHealth)
                {
                    healTarget = this;
                    heal = true;
                }
            }
        }
        currentAction = new Action(new Target(healTarget), (int)State.heal);
        anim.SetBool("Heal", true);
        healCooldown.Reset();
    }


    void Heal()
    {
        anim.SetBool("Heal", false);
        currentAction = new Action(new Target(), (int)State.move);
        healthCheck = null;
        if (healTarget != null)
        {
            healTarget.AddEffect(new HealEffect(healTarget, 3, 1, 1));
        }
    }
}
