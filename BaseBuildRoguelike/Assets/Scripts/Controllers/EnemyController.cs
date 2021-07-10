using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class EnemyController : MonoSingleton<EnemyController>
{
    [System.Serializable]
    public class EnemyTemplate
    {
        public GameObject prefab;
        public float rarity = 50;
    }

    public List<EnemyTemplate> enemyTemplates = new List<EnemyTemplate>();

    public List<Interaction> enemies = new List<Interaction>();
    public float spawnTime = 10;
    public List<Tile> corruptedTiles = new List<Tile>();

    public void StartSpawning()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(spawnTime / (1 + (corruptedTiles.Count / 100)));
        if (corruptedTiles.Count > 0)
        {
            Enemy enemy = EnemySelect();
            if (enemy != null)
            {
                enemies.Add(enemy);
            }
        }
        StartCoroutine(Spawn());
    }

    Enemy EnemySelect()
    {
        float random = Random.Range(0, 100);
        GameObject enemy = null;
        Vector3 spawnPos = corruptedTiles[Random.Range(0, corruptedTiles.Count)].transform.position;

        for (int i = 0; i < enemyTemplates.Count; i++)
        {
            if (random <= enemyTemplates[i].rarity)
            {
                enemy = Instantiate(enemyTemplates[i].prefab, spawnPos, Quaternion.identity);
            }
        }
        if (enemy == null) 
        {
            return null;
        }
        return enemy.GetComponent<Enemy>();
    }

    public void Remove(Enemy enemy)
    {
        enemies.Remove(enemy);
    }

    public void AddCorruptedTile(Tile tile)
    {
        corruptedTiles.Add(tile);
    }

    public void RemoveCorruptedTile(Tile tile)
    {
        corruptedTiles.Remove(tile);
    }
}
