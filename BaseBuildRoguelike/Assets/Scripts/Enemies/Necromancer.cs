using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Necromancer : Enemy
{
    public List<Corpse> corpses = new List<Corpse>();
    public List<Enemy> undead = new List<Enemy>();
    readonly Cooldown shotCooldown = new Cooldown(5), raiseCooldown = new Cooldown(10);
    public GameObject shadowBoltPrefab, necroSphere;
    private void Update()
    {
        if (raiseCooldown.Tick() && corpses.Count > 0)
        {
            raiseCooldown.Reset();
            StartCoroutine(RaiseDead());
        }

        if (shotCooldown.Tick() && target != null && target.interact)
        {
            shotCooldown.Reset();
            GameObject bolt = Instantiate(shadowBoltPrefab, transform.position, Quaternion.identity);
            bolt.GetComponent<Projectile>().Setup(target.interact, this, 5, 1);
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



}
