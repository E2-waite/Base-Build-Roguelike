using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoSingleton<Spawner>
{
    private bool start = false;
    public void Setup()
    {
        GameObject[] followerObjs = GameObject.FindGameObjectsWithTag("Follower");
        foreach (GameObject follower in followerObjs)
        {
            Followers.Add(follower.GetComponent<Follower>());
        }

        GameObject[] enemyObjs = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemyObjs)
        {
            Enemies.Add(enemy.GetComponent<Enemy>());
        }

        for (int i = 0; i < Resources.NUM; i++)
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
            if (enemySpawn.Tick())
            {
                enemySpawn = new Cooldown(spawnTime / (1 + (corruptedTiles.Count / 100)));
                SpawnEnemy();
            }
        }
    }

    [Header("Follower Settings")]
    public int followerCost = 10;
    public GameObject[] followerPrefab = new GameObject[4];
    public GameObject[] corpsePrefab = new GameObject[4];
    public void SpawnFollower(Vector3 pos)
    {
        GameObject follower = Instantiate(followerPrefab[0], Vector3.zero, Quaternion.identity).transform.GetChild(0).gameObject;
        follower.transform.position = pos;
        Followers.Add(follower.GetComponent<Worker>());
        HUD.Instance.UpdateFollowers();
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
            Vector3 spawnPos = corruptedTiles[Random.Range(0, corruptedTiles.Count)].transform.position;
            int num = Random.Range(0, enemyTemplates.Count);
            GameObject enemyObj = Instantiate(enemyTemplates[num].prefab, spawnPos, Quaternion.identity);
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            enemy.type = num;
            Enemies.Add(enemy);
        }
    }
    public void StartSpawning()
    {
        start = true;
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
    public List<BuildingTemplate> buildings = new List<BuildingTemplate>();
    public int selectedTemplate = 0;
    public void SpawnHome(Tile tile)
    {
        GameObject building = Instantiate(firepitPrefab, tile.transform.position, Quaternion.identity);
        tile.structure = building.GetComponent<Interaction>();
        Buildings.homeBase = tile.structure as HomeBase;
        (tile.structure as Building).tiles = new Vector2Int[1] { tile.GridPos() };
        Buildings.buildings.Add(Buildings.homeBase);
    }
    public void BuildStructure()
    {
        if (Build.CanBuild(buildings[selectedTemplate].type))
        {


            GameObject building = Instantiate(buildings[selectedTemplate].prefab, Vector3.zero, Quaternion.identity);
            Building structure = building.GetComponent<Building>();


            structure.tiles = new Vector2Int[Grid.selectedTiles.Count];
            for (int i = 0; i < Grid.selectedTiles.Count; i++)
            {
                Grid.selectedTiles[i].structure = structure;
                structure.tiles[i] = Grid.selectedTiles[i].GridPos();
            }

            structure.Centre();

            structure.type = selectedTemplate;
            Buildings.buildings.Add(structure);
            Pathfinding.UpdateNodeGrid();
        }
    }

    //Creatures
    [Header("Creatures Settings")]
    public GameObject rabbitPrefab;
    public int creatureScale = 10;
    public void SpawnCreatures()
    {
        Creatures.maxCreatures = creatureScale * (Grid.size / 10);
        for (int i = 0; i < Creatures.maxCreatures; i++)
        {
            Vector3 creaturePos = new Vector3((int)(Random.Range(0, Grid.size)), (int)(Random.Range(0, Grid.size)), 0);
            GameObject obj = Instantiate(rabbitPrefab, creaturePos, Quaternion.identity);
            Creature creature = obj.GetComponent<Creature>();
            Creatures.Add(creature);
            Vector2Int pos = creature.RandomTargetPos();
            creature.transform.position = new Vector3(pos.x, pos.y, 0);
        }
    }

    [Header("Projectile Settings")]
    public List<GameObject> projectilePrefab = new List<GameObject>();
}
