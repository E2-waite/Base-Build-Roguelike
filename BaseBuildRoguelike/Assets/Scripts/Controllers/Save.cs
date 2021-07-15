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
        List<Interaction> resources = Resources.trees;
        resources.AddRange(Resources.stones);

        for (int i = 0; i < resources.Count; i++)
        {
            Resource resource = resources[i] as Resource;
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
            Follower follower = Followers.followers[i] as Follower;
            if (follower != null)
            {
                Debug.Log(follower.name + " " + follower.Index());
            }
            else
            {
                Followers.followers.RemoveAt(i);
            }
            gameData.followers[i] = new FollowerData((int)follower.type, (int)follower.state, follower.target.Index(), follower.health, follower.transform.position.x, follower.transform.position.y);
        }
    }
}

[System.Serializable]
public class GameData
{
    public int mapSize, noise;
    public TileData[] tiles;
    public ResourceData[] resources;
    public BuildingData[] buildings;
    public FollowerData[] followers;
    public GameData(int size, int noiseVal, int numResources)
    {
        mapSize = size;
        noise = noiseVal;
        tiles = new TileData[size * size];
        resources = new ResourceData[numResources];
        buildings = new BuildingData[Buildings.buildings.Count];
        followers = new FollowerData[Followers.followers.Count];
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
public class FollowerData
{
    public int type, state, target, health;
    public float x, y;

    public FollowerData(int _type, int _state, int _target, int _health, float _x, float _y)
    {
        type = _type;
        state = _state;
        target = _target;
        health = _health;
        x = _x;
        y = _y;
    }
}

