using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : Interaction
{
    public Follower.Type type;
    public GameObject healthyPrefab, undeadPrefab;

    private void Start()
    {
        bool Boolean = (Random.value > 0.5f);
        if (Boolean)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        Followers.corpses.Add(this);
    }

    public void Revive(Priest priest)
    {

    }

    public void Revive(Necromancer necromancer)
    {
        GameObject undead = Instantiate(undeadPrefab, transform.position, Quaternion.identity);
        Enemy enemy = undead.GetComponent<Enemy>();
        Enemies.Add(enemy);
        necromancer.undead.Add(enemy);
        enemy.transform.localScale = transform.localScale;
        Followers.corpses.Remove(this);
        Destroy(gameObject);
    }
}
