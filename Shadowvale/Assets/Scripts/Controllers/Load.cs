using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Load : MonoBehaviour
{
    public GameObject squadPrefab;
    public bool LoadGame()
    {
        Debug.Log(Save.file);
        if (!System.IO.File.Exists(Application.persistentDataPath + "/" + Save.file + ".json"))
        {
            Debug.Log(Save.file + " Doesn't exist");
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
                if (Grid.tiles[x, y] != null)
                {
                    Grid.tiles[x, y].StartSpreading();
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
            if (buildingData.type > Spawner.Instance.buildings.Count)
            {
                buildingObj = Instantiate(Spawner.Instance.firepitPrefab, Grid.tiles[pos.x, pos.y].transform.position, Quaternion.identity);
                Buildings.homeBase = buildingObj.GetComponent<HomeBase>();
            }
            else
            {
                buildingObj = Instantiate(Spawner.Instance.buildings[buildingData.type].prefab, Grid.tiles[pos.x, pos.y].transform.position, Quaternion.identity);
            }

            Building building = buildingObj.GetComponent<Building>();
            Buildings.Add(building);

            if (building != null)
            {
                building.tiles = buildingData.tiles;
                for (int j = 0; j < building.tiles.Length; j++)
                {
                    Grid.tiles[building.tiles[j].x, building.tiles[j].y].structure = building;
                }
                building.Centre();
                building.type = buildingData.type;

                if (!(building is HomeBase))
                {
                    Construct construct = buildingObj.GetComponent<Construct>();
                    if (construct != null)
                    {
                        construct.CheckComplete(buildingData);
                    }

                    if (building is ResourceStorage)
                    {
                        (building as ResourceStorage).SetVal(buildingData.storage);
                    }
                    else if (building is House)
                    {
                        Followers.AdjustMaxFollowers(5);
                    }
                }
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
                Follower follower = followerObj.GetComponent<Follower>();
                follower.transform.position = followerData.pos;
                follower.health = followerData.health;
                follower.currentPos = followerData.gridPos;
                follower.marker.transform.position = followerData.markerPos;
                if (followerData.statusEffects != null)
                {
                    follower.statusEffects = followerData.statusEffects.Read(follower);
                    follower.glow.SetupGlow(follower.statusEffects);
                }
                else
                {
                    Debug.Log("NO STATUS EFFECT CLASS");
                }
                Followers.Add(follower);

                if (follower is Worker)
                {
                    (follower as Worker).inventory = followerData.inventory;
                }
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
                enemy.transform.position = enemyData.pos;
                enemy.currentPos = enemyData.gridPos;
                enemy.health = enemyData.health;
                if (enemyData.statusEffects != null)
                {
                    enemy.statusEffects = enemyData.statusEffects.Read(enemy);
                    if (enemy.glow != null)
                    {
                        enemy.glow.SetupGlow(enemy.statusEffects);
                    }
                }
                Enemies.Add(enemy);
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
            }
        }
        return SetTargets(gameData);
    }

    bool SetTargets(GameData gameData)
    {
        for (int i = 0; i < gameData.followers.Length; i++)
        {
            AIData aiData = gameData.followers[i];
            Followers.followers[i].actions = new List<Action>();
            Debug.Log(aiData.numActions);
            if (aiData.numActions > 0)
            {
                for (int j = 0; j < aiData.numActions; j++)
                {
                    Followers.followers[i].actions.Add(new Action(aiData.targets[j], aiData.states[j]));
                }
                Followers.followers[i].currentAction = Followers.followers[i].actions[Followers.followers[i].actions.Count - 1];
            }
            else
            {
                Followers.followers[i].currentAction = new Action();
            }
        }

        for (int i = 0; i < gameData.enemies.Length; i++)
        {
            AIData aiData = gameData.enemies[i];
            Enemies.enemies[i].actions = new List<Action>();
            if (aiData.numActions > 0)
            {
                for (int j = 0; j < aiData.numActions; j++)
                {
                    Enemies.enemies[i].actions.Add(new Action(aiData.targets[j], aiData.states[j]));
                }
                Enemies.enemies[i].currentAction = Enemies.enemies[i].actions[Enemies.enemies[i].actions.Count - 1];
            }
            else
            {
                Enemies.enemies[i].currentAction = new Action();
            }
        }

        Spawner.Instance.StartSpawning();

        return true;
    }
}
