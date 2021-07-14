using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : Interaction
{
    public GameObject healthyPrefab, undeadPrefab;

    private void Start()
    {
        bool Boolean = (Random.value > 0.5f);
        if (Boolean)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
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
        Destroy(gameObject);
    }
}
