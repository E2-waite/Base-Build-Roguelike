using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Necromancer : Enemy
{
    public List<Corpse> corpses = new List<Corpse>();
    public List<Enemy> undead = new List<Enemy>();
    public float cooldown = 10, currCooldown = 0;
    bool raisingDead = false;
    private void Update()
    {
        if (currCooldown <= 0 && corpses.Count > 0)
        {
            currCooldown = cooldown;
            StartCoroutine(RaiseDead());
        }

        if (currCooldown > 0)
        {
            currCooldown -= Time.deltaTime;
        }
    }

    IEnumerator RaiseDead()
    {
        raisingDead = true;
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < corpses.Count; i++)
        {
            if (corpses[i] != null)
            {
                corpses[i].Revive(this);
                yield return new WaitForSeconds(0.1f);
            }
        }
        raisingDead = false;
    }

}
