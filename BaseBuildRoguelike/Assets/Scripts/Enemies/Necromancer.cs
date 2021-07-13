using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Necromancer : Enemy
{
    public List<Corpse> corpses = new List<Corpse>();
    public List<Enemy> undead = new List<Enemy>();
    public float raiseCooldown = 10, currCooldown = 2, shotCooldown = 5, currShotCooldown = 0;
    bool raisingDead = false;
    public GameObject shadowBoltPrefab, necroSphere;
    private void Update()
    {
        if (currCooldown <= 0 && corpses.Count > 0)
        {
            currCooldown = raiseCooldown;
            StartCoroutine(RaiseDead());
        }

        if (currShotCooldown <= 0 && target != null)
        {
            currShotCooldown = shotCooldown;
            GameObject bolt = Instantiate(shadowBoltPrefab, transform.position, Quaternion.identity);
            bolt.GetComponent<Projectile>().Setup(target, this, 5, 1);
        }

        if (currCooldown > 0)
        {
            currCooldown -= Time.deltaTime;
        }

        if (currShotCooldown > 0)
        {
            currShotCooldown -= Time.deltaTime;
        }
    }

    IEnumerator RaiseDead()
    {
        raisingDead = true;
        yield return new WaitForSeconds(0.1f);

        for (int i = corpses.Count - 1; i >= 0 ; i--)
        {
            Debug.Log(i.ToString() + corpses[i].name);
            if (corpses[i] != null)
            {
                GameObject projectile = Instantiate(necroSphere, transform.position, Quaternion.identity);
                projectile.GetComponent<Projectile>().Setup(corpses[i], this, 5);
                yield return new WaitForSeconds(0.1f);
            }
        }
        raisingDead = false;
    }



}
