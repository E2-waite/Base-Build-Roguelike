using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Necromancer : Enemy
{
    public List<Corpse> corpses = new List<Corpse>();
    public List<Enemy> undead = new List<Enemy>();
    public Cooldown shotCooldown = new Cooldown(5), raiseCooldown = new Cooldown(10);
    public GameObject shadowBoltPrefab, necroSphere;
    private void Update()
    {
        Swarm();
        if (currentAction.target.interact == null)
        {
            PreviousTarget();
        }
        else
        {
            Move();
        }

        if (raiseCooldown.Tick() && corpses.Count > 0)
        {
            raiseCooldown.Reset();
            StartCoroutine(RaiseDead());
        }

        if (shotCooldown.Tick() && currentAction != null && currentAction.target.interact)
        {
            shotCooldown.Reset();
            GameObject bolt = Instantiate(shadowBoltPrefab, transform.position, Quaternion.identity);
            bolt.GetComponent<Projectile>().Setup(currentAction.target.interact, this, 5, 1);
        }

    }

    IEnumerator RaiseDead()
    {
        yield return new WaitForSeconds(0.1f);

        for (int i = corpses.Count - 1; i >= 0 ; i--)
        {
            if (corpses[i] != null)
            {
                GameObject projectile = Instantiate(necroSphere, transform.position, Quaternion.identity);
                projectile.GetComponent<Projectile>().Setup(corpses[i], this, 5);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }


    public override void Save(AIData data)
    {
        base.Save(data);
        data.cooldowns = new Cooldown[2] { shotCooldown, raiseCooldown };
    }

    public override void Load(AIData data)
    {
        base.Load(data);
        shotCooldown = data.cooldowns[0];
        raiseCooldown = data.cooldowns[1];
    }
}
