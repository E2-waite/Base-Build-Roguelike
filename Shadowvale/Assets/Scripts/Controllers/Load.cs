using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Load : MonoBehaviour
{
    public GameObject friendlySquad, hostileSquad;
    public bool LoadGame()
    {
        if (!System.IO.File.Exists(Application.persistentDataPath + "/" + Save.file + ".json"))
        {
            return false;
        }

        string jsonString = File.ReadAllText(Application.persistentDataPath + "/" + Save.file + ".json");
        GameData gameData = JsonUtility.FromJson<GameData>(jsonString);

        GameController.Instance.gameCam.transform.position = new Vector3(gameData.camX, gameData.camY, GameController.Instance.gameCam.transform.position.z);

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
                Grid.tiles[tileData.x, tileData.y].Setup(tileData.corruption, tileData.multi, new Vector2Int(tileData.x, tileData.y));
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
                if (Grid.tiles[x, y] != null)
                {
                    Grid.tiles[x, y].UpdateSprite(x, y);
                }
            }
        }

        return LoadResources(gameData);
    }

    bool LoadResources(GameData gameData)
    {
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
            Resources.allResources.Add(Grid.tiles[pos.x, pos.y].structure);

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
            Vector2Int pos = new Vector2Int(buildingData.tiles[0].x, buildingData.tiles[0].y);
            GameObject buildingObj;
            if (buildingData.type >= 0)
            {
                buildingObj = Instantiate(Spawner.Instance.buildings[buildingData.type].prefab, Grid.tiles[pos.x, pos.y].transform.position, Quaternion.identity);
            }
            else
            {
                buildingObj = Instantiate(Spawner.Instance.firepitPrefab, Grid.tiles[pos.x, pos.y].transform.position, Quaternion.identity);
                Buildings.homeBase = buildingObj.GetComponent<HomeBase>();
            }

            Building building = buildingObj.GetComponent<Building>();

            if (building != null)
            {
                building.Load(buildingData);
            }
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
                Follower follower = followerObj.GetComponent<Follower>();
                follower.Load(followerData);
            }
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
                enemy.Load(enemyData);
            }
        }
        return LoadCreatures(gameData);
    }

    bool LoadCreatures(GameData gameData)
    {
        for (int i = 0; i < gameData.creatures.Length; i++)
        {
            CreatureData creatureData = gameData.creatures[i];
            GameObject creatureObj = Instantiate(Spawner.Instance.rabbitPrefab, creatureData.pos, Quaternion.identity);
            Creature creature = creatureObj.GetComponent<Creature>();
            Creatures.Add(creature);
            creature.health = creatureData.health;
            creature.startPos = creatureData.startPos;
        }
        return LoadCorpses(gameData);
    }

    bool LoadCorpses(GameData gameData)
    {
        for (int i = 0; i < gameData.corpses.Length; i++)
        {
            CorpseData corpseData = gameData.corpses[i];
            GameObject corpseObj = Instantiate(Spawner.Instance.corpsePrefab[corpseData.type], corpseData.pos, Quaternion.identity);
            if (corpseData.flipped)
            {
                corpseObj.transform.localScale = new Vector2(-1, 1);
            }
            Followers.corpses.Add(corpseObj.GetComponent<Corpse>());
        }
        return LoadSquads(gameData);
    }

    bool LoadSquads(GameData gameData)
    {
        // need to have instantiate of friendly and hostile squads
        for (int i = 0; i < gameData.squads.Length; i++)
        {
            SquadData squadData = gameData.squads[i];
            GameObject squadObj = null;
            if (squadData.friendly)
            {
                squadObj = Instantiate(friendlySquad, Vector3.zero, Quaternion.identity);
            }
            else
            {
                squadObj = Instantiate(hostileSquad, Vector3.zero, Quaternion.identity);
            }

            if (squadObj != null)
            {
                Squad squad = squadObj.GetComponent<Squad>();
                squad.Setup(squadData.members);
            }
        }
        return SetTargets(gameData);
    }

    bool SetTargets(GameData gameData)
    {
        for (int i = 0; i < Followers.followers.Count; i++)
        {
            Followers.followers[i].SetTargets();
        }

        for (int i = 0; i < Enemies.enemies.Count; i++)
        {
            Enemies.enemies[i].SetTargets();
        }

        Spawner.Instance.StartSpawning();
        return LoadBuildingVals(gameData);
    }

    bool LoadBuildingVals(GameData gameData)
    {
        for (int i = 0; i < Buildings.buildings.Count; i++)
        {
            Building building = Buildings.buildings[i];
            if (building.buildingData != null)
            {
                building.LoadInstance();
            }
        }
        return LoadProjectiles(gameData);
    }

    bool LoadProjectiles(GameData gameData)
    {
        for (int i = 0; i < gameData.projectiles.Length; i++)
        {
            ProjectileData projectileData = gameData.projectiles[i];
            GameObject projectileObj = Instantiate(Spawner.Instance.projectilePrefab[projectileData.type], projectileData.pos, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();

            projectile.Setup(Grid.TargetFromIndex(projectileData.target), (projectileData.origin >= 0) ? Grid.TargetFromIndex(projectileData.origin) : null, projectileData.speed, projectileData.damage);
        }
        return true;
    }
}
