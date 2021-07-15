using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class Save : MonoBehaviour
{
    public void SaveGame()
    {
        GameData gameData = new GameData(Grid.size, Grid.noise, Resources.trees.Count + Resources.stones.Count);

        SaveTiles(gameData);
        SaveResources(gameData);
        SaveBuildings(gameData);
        SaveFollowers(gameData);
        SaveEnemies(gameData);
        SaveSquads(gameData);

        List<Interaction> interactable = Grid.GetAllInteractable();

        System.IO.File.WriteAllText(Application.persistentDataPath + "/SaveData.json", JsonUtility.ToJson(gameData));
    }

    private void SaveTiles(GameData gameData)
    {
        // Serialize tiles
        int ind = 0;
        for (int y = 0; y < Grid.size; y++)
        {
            for (int x = 0; x < Grid.size; x++)
            {
                Tile tile = Grid.GetTile(new Vector2Int(x, y));
                gameData.tiles[ind++] = new TileData((int)tile.type, tile.corruptionVal, x, y);
                //tilesString += JsonUtility.ToJson(data);
            }
        }
    }

    private void SaveResources(GameData gameData)
    {
        // Serialize resources
        for (int i = 0; i < Resources.allResources.Count; i++)
        {
            Resource resource = Resources.allResources[i] as Resource;
            gameData.resources[i] = new ResourceData((int)resource.type, resource.val, (int)resource.transform.position.x, (int)resource.transform.position.y);
        }
    }

    private void SaveBuildings(GameData gameData)
    {
        for (int i = 0; i < Buildings.buildings.Count; i++)
        {
            Building building = Buildings.buildings[i];
            gameData.buildings[i] = new BuildingData(building.type, 0, 0, building.repair, (int)building.transform.position.x, (int)building.transform.position.y);
        }
    }

    private void SaveFollowers(GameData gameData)
    {
        for (int i = 0; i < Followers.followers.Count; i++)
        {
            Follower follower = Followers.followers[i];
            if (follower != null)
            {
                int targetInd = 99999;
                if (follower.target != null)
                {
                    targetInd = follower.target.Index();
                }
                gameData.followers[i] = new AIData((int)follower.type, (int)follower.state, targetInd, follower.health, follower.transform.position.x, follower.transform.position.y);
            }
            else
            {
                Followers.followers.RemoveAt(i);
            }
        }
    }

    private void SaveEnemies(GameData gameData)
    {
        for (int i = 0; i < Enemies.enemies.Count; i++)
        {
            Enemy enemy = Enemies.enemies[i];
            if (enemy != null)
            {
                int targetInd = 99999;
                if (enemy.target != null)
                {
                    targetInd = enemy.target.Index();
                }
                gameData.enemies[i] = new AIData((int)enemy.type, 0, targetInd, enemy.health, enemy.transform.position.x, enemy.transform.position.y);
            }
            else
            {
                Enemies.enemies.RemoveAt(i);
            }
        }
    }

    private void SaveSquads(GameData gameData)
    {
        List<Squad> allSquads = Followers.squads;
        allSquads.AddRange(Enemies.squads);
        for (int i = 0; i < allSquads.Count; i++)
        {
            Squad squad = allSquads[i];
            int[] members = SquadMembersIndexes(squad);
            int targetInd = 99999;
            if (squad.target != null)
            {
                targetInd = squad.target.Index();
            }
            gameData.squads[i] = new SquadData(members, 0, targetInd, squad.marker.transform.position.x, squad.marker.transform.position.y);
        }
    }

    int[] SquadMembersIndexes(Squad squad)
    {
        int[] memberIndexes = new int[squad.members.Count];
        for (int i = 0; i < squad.members.Count; i++)
        {
            memberIndexes[i] = squad.members[i].Index();
        }
        return memberIndexes;
    }
}

[System.Serializable]
public class GameData
{
    public int mapSize, noise;
    public TileData[] tiles;
    public ResourceData[] resources;
    public BuildingData[] buildings;
    public AIData[] followers, enemies;
    public SquadData[] squads;
    public GameData(int size, int noiseVal, int numResources)
    {
        mapSize = size;
        noise = noiseVal;
        tiles = new TileData[size * size];
        resources = new ResourceData[numResources];
        buildings = new BuildingData[Buildings.buildings.Count];
        followers = new AIData[Followers.followers.Count];
        enemies = new AIData[Enemies.enemies.Count];
        squads = new SquadData[Followers.squads.Count + Enemies.squads.Count];
    }
}

[System.Serializable]
public class TileData
{
    public int type;
    public float corruption;
    public int x, y;
    public TileData(int _type, float _corruption, int _x, int _y)
    {
        type = _type;
        corruption = _corruption;
        x = _x;
        y = _y;
    }
}

[System.Serializable]
public class ResourceData
{
    public int type, val, x, y;

    public ResourceData(int _type, int _val, int _x, int _y)
    {
        type = _type;
        val = _val;
        x = _x;
        y = _y;
    }
}

[System.Serializable]
public class BuildingData
{
    public int type, woodLeft, stoneLeft, health, x, y;

    public BuildingData(int _type, int _woodLeft, int _stoneLeft, int _health, int _x, int _y)
    {
        type = _type;
        woodLeft = _woodLeft;
        stoneLeft = _stoneLeft;
        health = _health;
        x = _x;
        y = _y;
    }
}

[System.Serializable]
public class AIData
{
    public int type, state, target, health;
    public float x, y;

    public AIData(int _type, int _state, int _target, int _health, float _x, float _y)
    {
        type = _type;
        state = _state;
        target = _target;
        health = _health;
        x = _x;
        y = _y;
    }
}

[System.Serializable]
public class SquadData
{
    public int[] members;
    public int state, target;
    public float x, y;

    public SquadData(int[] _members, int _state, int _target, float _x, float _y)
    {
        members = _members;
        state = _state;
        target = _target;
        x = _x;
        y = _y;
    }
}
