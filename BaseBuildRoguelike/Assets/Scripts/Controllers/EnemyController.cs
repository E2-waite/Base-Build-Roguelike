using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoSingleton<EnemyController>
{
    public GameObject prefab;
    public float spawnTime = 1;
    public List<Enemy> enemies = new List<Enemy>();
    public void StartSpawning()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        yield return new WaitForSeconds(spawnTime);
        GameObject enemy = Instantiate(prefab, new Vector3(Random.Range(0, GameController.Instance.grid.mapSize), Random.Range(0, GameController.Instance.grid.mapSize), 0), Quaternion.identity);
        enemies.Add(enemy.GetComponent<Enemy>());
        StartCoroutine(Spawn());
    }

    public void Remove(Enemy enemy)
    {
        enemies.Remove(enemy);
    }
}
