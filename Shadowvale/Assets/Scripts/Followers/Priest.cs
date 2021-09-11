using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Priest : Support
{
    Cooldown healCooldown = new Cooldown(5);

    public override void Update()
    {
        healCooldown.Tick();
        base.Update();
    }

    public override void Setup()
    {
        //StartCoroutine(HealthCheck());
    }

    public override bool FindTarget()
    {
        if (squad != null)
        {
            Follower mostDamaged = null;
            int highestDamage = 0;
            for (int i = 0; i < squad.members.Count; i++)
            {
                Follower member = squad.members[i] as Follower;
                if (member.maxHealth - member.health > highestDamage)
                {
                    mostDamaged = member;
                    highestDamage = member.maxHealth - member.health;
                }
            }

            if (mostDamaged != null)
            {
                actions.Add(new Action(new Target(mostDamaged), (int)SupportState.support));
                currentAction = actions[actions.Count - 1];
                return true;
            }
            return false;
        }
        else if (health < maxHealth)
        {
            actions.Add(new Action(new Target(this), (int)SupportState.support));
            currentAction = actions[actions.Count - 1];
            return true;
        }
        return false;
    }

    public override bool SupportFollower()
    {
        if (healCooldown.Complete() && currentAction.target != null)
        {
            Follower follower = currentAction.target.interact as Follower;
            follower.AddEffect(new HealEffect(follower, 3, 1, 1));
            healCooldown.Reset();
            StartCoroutine(HealRoutine());
            return true;
        }
        return false;
    }


    IEnumerator HealRoutine()
    {
        anim.SetBool("Heal", true);
        yield return new WaitForSeconds(0.01f);
        anim.SetBool("Heal", false);
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
