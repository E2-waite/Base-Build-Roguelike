using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Priest : Support
{
    public int healAmount = 5;
    public float healRange = 5;
    public bool canHeal = true, healing = false;
    Cooldown healCooldown = new Cooldown(5);
    Coroutine healthCheck = null;
    Follower healTarget = null;
    public override void Update()
    {
        if (healCooldown.Tick() && healthCheck == null)
        {
            // If cooled down and not currently checking health
            healthCheck = StartCoroutine(HealthCheck());
        }

        base.Update();
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
        currentAction = new Action(new Target(healTarget), (int)SupportState.heal);
        anim.SetBool("Heal", true);
        healCooldown.Reset();
    }


    void Heal()
    {
        anim.SetBool("Heal", false);
        currentAction = new Action(new Target(), (int)SupportState.move);
        healthCheck = null;
        if (healTarget != null)
        {
            healTarget.AddEffect(new HealEffect(healTarget, 3, 1, 1));
        }
    }

    public override void Save(AIData data)
    {
        base.Save(data);
        data.cooldowns = new Cooldown[1] { healCooldown };
    }

    public override void Load(AIData data)
    {
        base.Load(data);
        healCooldown = data.cooldowns[0];
    }
}
