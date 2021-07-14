using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoSingleton<Spawner>
{
    bool start = false;
    public void Setup()
    {
        GameObject[] followerObjs = GameObject.FindGameObjectsWithTag("Follower");
        foreach (GameObject follower in followerObjs)
        {
            Followers.Add(follower.GetComponent<Interaction>());
        }

        GameObject[] enemyObjs = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemyObjs)
        {
            Enemies.Add(enemy.GetComponent<Interaction>());
        }

        for (int i = 0; i < Consts.NUM_RESOURCES; i++)
        {
            Buildings.storages[i] = new List<ResourceStorage>();
        }
        Buildings.walls = new Wall[Grid.size, Grid.size];

        Creatures.maxCreatures = creatureScale * (Grid.size / 10);
        start = true;
    }

 
    private void Update()
    {
        if (start)
        {
            enemySpawn.Tick();
            if (enemySpawn.Complete())
            {
                enemySpawn = new Cooldown(spawnTime / (1 + (corruptedTiles.Count / 100)));
                SpawnEnemy();
            }
        }
    }

    [Header("Follower Settings")]
    public int followerCost = 10;
    public GameObject followerPrefab;
    public void SpawnFollower(Vector3 pos)
    {
        GameObject follower = Instantiate(followerPrefab, pos, Quaternion.identity);
        Followers.Add(follower.GetComponent<Worker>());
        HUD.Instance.UpdateFollowers(Followers.Count(), Followers.Max());
    }


    [System.Serializable]
    public class EnemyTemplate
    {
        public GameObject prefab;
        public float rarity = 50;
    }
    [Header("Enemy Settings")]
    public List<EnemyTemplate> enemyTemplates = new List<EnemyTemplate>();
    public List<Tile> corruptedTiles = new List<Tile>();
    Cooldown enemySpawn = new Cooldown(10);
    public float spawnTime = 10;
    public void SpawnEnemy()
    {
        if (corruptedTiles.Count > 0)
        {
            Enemy enemy = EnemySelect();
            if (enemy != null)
            {
                Enemies.Add(enemy);
            }
        }
    }
    Enemy EnemySelect()
    {
        Vector3 spawnPos = corruptedTiles[Random.Range(0, corruptedTiles.Count)].transform.position;
        GameObject enemy = Instantiate(enemyTemplates[Random.Range(0, enemyTemplates.Count)].prefab, spawnPos, Quaternion.identity);
        return enemy.GetComponent<Enemy>();
    }
    public void AddCorruptedTile(Tile tile)
    {
        corruptedTiles.Add(tile);
    }
    public void RemoveCorruptedTile(Tile tile)
    {
        corruptedTiles.Remove(tile);
    }

    // Buildings
    [System.Serializable]
    public class BuildingTemplate
    {
        public string name;
        public GameObject prefab;
        public Sprite sprite;
        public Build.Type type = Build.Type.standard;
    }
    [Header("Building Settings")]
    public GameObject firepitPrefab;
    public List<BuildingTemplate> buildingTemplates = new List<BuildingTemplate>();
    public BuildingTemplate selectedTemplate = null;
    public void SpawnHome(Tile tile)
    {
        GameObject building = Instantiate(firepitPrefab, tile.transform.position, Quaternion.identity);
        tile.structure = building.GetComponent<Interaction>();
        Buildings.homeBase = tile.structure as HomeBase;
    }
    public void BuildStructure(Tile tile)
    {
        if (tile != null && tile.structure == null && selectedTemplate != null)
        {
            GameObject building = Instantiate(selectedTemplate.prefab, tile.transform.position, Quaternion.identity);
            tile.structure = building.GetComponent<Interaction>();
            tile.structure.transform.parent = tile.transform;
            Pathfinding.UpdateNodeGrid();
        }
    }

    //Creatures
    [Header("Creatures Settings")]
    public GameObject rabbitPrefab;
    public int creatureScale = 10;
    public void SpawnCreatures(int mapSize)
    {
        for (int i = 0; i < Creatures.maxCreatures; i++)
        {
            Vector3 creaturePos = new Vector3((int)(Random.Range(0, mapSize)), (int)(Random.Range(0, mapSize)), 0);
            GameObject creature = Instantiate(rabbitPrefab, creaturePos, Quaternion.identity);
            Creatures.Add(creature.GetComponent<Interaction>());
        }
    }
}
