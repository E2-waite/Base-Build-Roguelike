using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Grid
{
    public static Tile[,] tiles;
    public static Tile selected = null;
    public static int size, noise;
    public static Vector2Int startPos;
    public static void Init(int mapSize, int noiseSize)
    {
        size = mapSize;
        noise = noiseSize;
        startPos = new Vector2Int(size / 2, size / 2);
        tiles = new Tile[size, size];
    }

    public static bool IsSelected(Collider2D tile)
    {
        Vector2Int arrayPos = new Vector2Int((int)(tile.transform.position.x), (int)(tile.transform.position.y));
        if (selected == tiles[arrayPos.x, arrayPos.y])
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void SelectTile(Collider2D tile, Spawner.BuildingTemplate buildingTemplate)
    {
        Vector2Int arrayPos = new Vector2Int((int)(tile.transform.position.x), (int)(tile.transform.position.y));
        if (selected != null)
        {
            selected.Deselect();
            selected = null;
        }

        selected = tiles[arrayPos.x, arrayPos.y];
        selected.Select(Build.CanBuild(buildingTemplate.type, arrayPos));
    }

    public static void DeselectTile()
    {
        if (selected != null)
        {
            selected.Deselect();
            selected = null;
        }
    }

    public static bool InGrid(Vector2Int pos)
    {
        if (pos.x >= 0 && pos.x < size && pos.y > 0 && pos.y < size)
        {
            return true;
        }
        return false;
    }

    public static Tile GetTile(Vector2Int pos)
    {
        return tiles[pos.x, pos.y];
    }

    public static List<Interaction> GetAllInteractable()
    {
        List<Interaction> interactions = new List<Interaction>();
        interactions.AddRange(Resources.trees);
        interactions.AddRange(Resources.stones);
        interactions.AddRange(Buildings.buildings);
        interactions.AddRange(Followers.followers);
        interactions.AddRange(Enemies.enemies);
        interactions.AddRange(Creatures.creatures);
        return interactions;
    }
}
