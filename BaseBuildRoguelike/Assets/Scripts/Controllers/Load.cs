using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Load : MonoBehaviour
{
    public GameObject squadPrefab;
    public bool LoadGame()
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + "/SaveData.json"))
        {
            return false;
        }

        string jsonString = File.ReadAllText(Application.persistentDataPath + "/SaveData.json");
        GameData gameData = JsonUtility.FromJson<GameData>(jsonString);
        return LoadTiles(gameData);
    }

    bool LoadTiles(GameData gameData)
    {
        Grid.Init(gameData.mapSize, gameData.noise);
        for (int i = 0; i < gameData.tiles.Length; i++)
        {
            TileData tileData = gameData.tiles[i];
            Vector2 pos = new Vector2(tileData.x, tileData.y);
            GameObject tile = null;
            Tile.Type type = (Tile.Type)tileData.type;
            if (type == Tile.Type.water)
            {
                tile = Instantiate(GridBuilder.Instance.waterTile, pos, Quaternion.identity);
            }
            else if (type == Tile.Type.sand)
            {
                tile = Instantiate(GridBuilder.Instance.sandTile, pos, Quaternion.identity);
            }
            else if (type == Tile.Type.grass)
            {
                tile = Instantiate(GridBuilder.Instance.grassTile, pos, Quaternion.identity);
            }
            else if (type == Tile.Type.darkGrass)
            {
                tile = Instantiate(GridBuilder.Instance.dGrassTile, pos, Quaternion.identity);
            }

            if (tile != null)
            {
                Grid.tiles[tileData.x, tileData.y] = tile.GetComponent<Tile>();
                Grid.tiles[tileData.x, tileData.y].Setup(tileData.corruption);
                if (tileData.corruption >= 100)
                {
                    Spawner.Instance.corruptedTiles.Add(Grid.tiles[tileData.x, tileData.y]);
                }
            }
        }

        for (int y = 0; y < Grid.size; y++)
        {
            for (int x = 0; x < Grid.size; x++)
            {
                Grid.tiles[x, y].StartSpreading();
                Grid.tiles[x, y].UpdateSprite(x, y);
            }
        }

        return LoadResources(gameData);
    }

    bool LoadResources(GameData gameData)
    {
        Debug.Log("Resources: " + gameData.resources.Length.ToString());
        for (int i = 0; i < gameData.resources.Length; i++)
        {
            ResourceData resourceData = gameData.resources[i];
            Vector2Int pos = new Vector2Int(resourceData.x, resourceData.y);
            GameObject resource = null;
            Resource.Type type = (Resource.Type)gameData.resources[i].type;
            if (type == Resource.Type.wood)
            {
                resource = Instantiate(GridBuilder.Instance.treePrefab, Grid.tiles[pos.x, pos.y].transform.position, Quaternion.identity);
            }
            else if (type == Resource.Type.stone)
            {
                resource = Instantiate(GridBuilder.Instance.stonePrefab, Grid.tiles[pos.x, pos.y].transform.position, Quaternion.identity);
            }

            if (resource != null)
            {
                Grid.tiles[pos.x, pos.y].structure = resource.GetComponent<Interaction>();
            }

            if (type == Resource.Type.wood)
            {
                Resources.trees.Add(Grid.tiles[pos.x, pos.y].structure);
            }
            else if (type == Resource.Type.stone)
            {
                Resources.stones.Add(Grid.tiles[pos.x, pos.y].structure);
            }

        }
        return LoadBuildings(gameData);
    }

    bool LoadBuildings(GameData gameData)
    {
        for (int i = 0; i < Resources.NUM; i++)
        {
            Buildings.storages[i] = new List<ResourceStorage>();
        }
        Buildings.walls = new Wall[Grid.size, Grid.size];

        for (int i = 0; i < gameData.buildings.Length; i++)
        {
            BuildingData buildingData = gameData.buildings[i];
            Vector2Int pos = new Vector2Int(buildingData.x, buildingData.y);
            GameObject building = null;
            if (gameData.buildings[i].type == 99)
            {
                building = Instantiate(Spawner.Instance.firepitPrefab, Grid.tiles[pos.x, pos.y].transform.position, Quaternion.identity);
                Buildings.homeBase = building.GetComponent<HomeBase>();
            }
            else
            {
                building = Instantiate(Spawner.Instance.buildings[buildingData.type].prefab, Grid.tiles[pos.x, pos.y].transform.position, Quaternion.identity);
            }
            Buildings.Add(building.GetComponent<Building>());
            if (building != null && gameData.buildings[i].type != 99)
            {
                Grid.tiles[pos.x, pos.y].structure = building.GetComponent<Interaction>();
                Construct construct = building.GetComponent<Construct>();
                construct.woodRemaining = gameData.buildings[i].woodLeft;
                construct.stoneRemaining = gameData.buildings[i].stoneLeft;
            }

            // Add to building list based on type

        }
        Pathfinding.UpdateNodeGrid();
        return LoadFollowers(gameData);
    }

    bool LoadFollowers(GameData gameData)
    {
        for (int i = 0; i < gameData.followers.Length; i++)
        {
            AIData followerData = gameData.followers[i];
            GameObject followerObj = Instantiate(Spawner.Instance.followerPrefab[followerData.type], new Vector3(0, 0, 0), Quaternion.identity).transform.GetChild(0).gameObject;

            if (followerObj != null)
            {
                Debug.Log(followerObj.name);
                Follower follower = followerObj.GetComponent<Follower>();
                follower.transform.position = new Vector3(followerData.x, followerData.y, 0);
                follower.health = followerData.health;
                follower.state = followerData.state;
                Followers.Add(follower);
            }

            // Add to building list based on type

        }
        return LoadEnemies(gameData);
    }

    bool LoadEnemies(GameData gameData)
    {
        for (int i = 0; i < gameData.enemies.Length; i++)
        {
            AIData enemyData = gameData.enemies[i];
            GameObject enemyObj = Instantiate(Spawner.Instance.enemyTemplates[enemyData.type].prefab, new Vector3(0, 0, 0), Quaternion.identity);

            if (enemyObj != null)
            {
                Enemy enemy = enemyObj.GetComponent<Enemy>();
                enemy.transform.position = new Vector3(enemyData.x, enemyData.y, 0);
                enemy.health = enemyData.health;
                Enemies.Add(enemy);
            }
        }
        return LoadSquads(gameData);
    }

    bool LoadSquads(GameData gameData)
    {
        for (int i = 0; i < gameData.squads.Length; i++)
        {
            SquadData squadData = gameData.squads[i];
            GameObject squadObj = Instantiate(squadPrefab, Vector3.zero, Quaternion.identity);

            if (squadObj != null)
            {
                Squad squad = squadObj.GetComponent<Squad>();
                squad.Setup(squadData.members);
                squad.target = Grid.TargetFromIndex(squadData.target);
                squad.marker.transform.position = new Vector3(squadData.x, squadData.y, 0);
            }
        }
        return SetTargets(gameData);
    }

    bool SetTargets(GameData gameData)
    {
        for (int i = 0; i < gameData.followers.Length; i++)
        {
            if (gameData.followers[i].target != 99999)
            {
                Followers.followers[i].target = Grid.TargetFromIndex(gameData.followers[i].target);
            }
        }

        for (int i = 0; i < gameData.enemies.Length; i++)
        {
            if (gameData.enemies[i].target != 99999)
            {
                Enemies.enemies[i].target = Grid.TargetFromIndex(gameData.enemies[i].target);
            }
        }

        Spawner.Instance.StartSpawning();

        return true;
    }
}
