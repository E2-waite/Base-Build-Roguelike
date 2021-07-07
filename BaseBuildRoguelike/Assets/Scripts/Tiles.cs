using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tiles
{
    private static Grid grid;
    public static int mapSize;
    public static void Setup(Grid startGrid, int startSize)
    {
        grid = startGrid;
        mapSize = startSize;
    }

    public static Vector2Int[] NeighbourPositions(Vector2Int pos)
    {
        return new Vector2Int[4] { new Vector2Int(pos.x, pos.y + 1), new Vector2Int(pos.x + 1, pos.y), new Vector2Int(pos.x, pos.y - 1), new Vector2Int(pos.x - 1, pos.y) };
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
