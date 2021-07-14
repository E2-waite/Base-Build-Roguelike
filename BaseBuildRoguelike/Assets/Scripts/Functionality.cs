using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooldown
{
    private float max, current = 0;
    public Cooldown(float time)
    {
        max = time;
        current = max;
    }

    public void Tick()
    {
        if (current > 0)
        {
            current -= Time.deltaTime;
        }
    }

    public void Reset()
    {
        current = max;
    }

    public bool Complete()
    {
        if (current <= 0)
        {
            return true;
        }
        return false;
    }
    
}

public class Params
{
    public static Vector2Int[] Get4Neighbours(Vector2Int pos)
    {
        return new Vector2Int[4] { new Vector2Int(pos.x, pos.y + 1), new Vector2Int(pos.x + 1, pos.y), new Vector2Int(pos.x, pos.y - 1), new Vector2Int(pos.x - 1, pos.y) };
    }

    public static Vector2Int[] Get8Neighbours(Vector2Int pos)
    {
        return new Vector2Int[8] { new Vector2Int(pos.x, pos.y + 1), new Vector2Int(pos.x + 1, pos.y + 1), new Vector2Int(pos.x + 1, pos.y), new Vector2Int(pos.x + 1, pos.y - 1), 
            new Vector2Int(pos.x, pos.y - 1), new Vector2Int(pos.x - 1, pos.y - 1), new Vector2Int(pos.x - 1, pos.y), new Vector2Int(pos.x - 1, pos.y + 1) };
    }
}

public static class Tiles
{
    private static Grid grid;
    public static int mapSize;
    public static void Setup(Grid startGrid, int startSize)
    {
        grid = startGrid;
        mapSize = startSize;
    }

    public static bool InGrid(Vector2Int pos)
    {
        if (pos.x >= 0 && pos.x < grid.mapSize && pos.y > 0 && pos.y < grid.mapSize)
        {
            return true;
        }
        return false;
    }

    public static Tile GetTile(Vector2Int pos)
    {
        return grid.tiles[pos.x, pos.y];
    }
}

