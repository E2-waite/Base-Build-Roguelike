using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Grid
{
    public static Tile[,] tiles;
    //public static Tile selected = null;
    public static List<Tile> selectedTiles = new List<Tile>();
    public static Vector2Int selectedPos;
    public static int size, noise;
    public static Vector2Int startPos;
    public static void Init(int mapSize, int noiseSize)
    {
        size = mapSize;
        noise = noiseSize;
        startPos = new Vector2Int(size / 2, size / 2);
        tiles = new Tile[size, size];
    }

    public static bool IsSelected(Vector3 tile)
    {
        if (selectedTiles.Count > 0)
        {
            Vector2Int arrayPos = new Vector2Int((int)(tile.x), (int)(tile.y));
            if (arrayPos == selectedPos)
            {
                return true;
            }
        }
        return false;
    }

    public static void SelectTile(Vector3 tile, Spawner.BuildingTemplate buildingTemplate)
    {
        selectedPos = new Vector2Int((int)(tile.x), (int)(tile.y));
        for (int i = 0; i < selectedTiles.Count; i++)
        {
            selectedTiles[i].Deselect();
        }
        selectedTiles = new List<Tile>();

        selectedTiles.Add(tiles[selectedPos.x, selectedPos.y]);
        tiles[selectedPos.x, selectedPos.y].Select(Build.CanBuild(buildingTemplate.type));
    }

    // Selects tiles in a 2x2 grid 
    public static void SelectTiles(Vector3 tilePos, Spawner.BuildingTemplate buildingTemplate)
    {
        selectedPos = new Vector2Int((int)tilePos.x, (int)tilePos.y);

        for (int i = 0; i < selectedTiles.Count; i++)
        {
            selectedTiles[i].Deselect();
        }


        Vector2Int startPos = new Vector2Int((tilePos.x % 1 < 0.5f) ? selectedPos.x - 1 : selectedPos.x, (tilePos.y % 1 < 0.5f) ? selectedPos.y - 1 : selectedPos.y);
        selectedTiles = new List<Tile>();
        for (int y = 0; y < 2; y++)
        {
            for (int x = 0; x < 2; x++)
            {
                Vector2Int pos = new Vector2Int(startPos.x + x, startPos.y + y);
                if (InGrid(pos))
                {
                    Tile tile = tiles[pos.x, pos.y];
                    selectedTiles.Add(tile);
                }
            }
        }

        bool canBuild = Build.CanBuild(buildingTemplate.type);
        Debug.Log((canBuild) ? "Can Build" : "Can't Build");
        for (int i = 0; i < selectedTiles.Count; i++)
        {
            Tile tile = selectedTiles[i];
            tile.Select(canBuild);
        }
    }

    public static void DeselectTile()
    {
        for (int i = 0; i < selectedTiles.Count; i++)
        {
            selectedTiles[i].Deselect();
        }
        selectedTiles = new List<Tile>();
    }

    public static bool InGrid(Vector2Int pos)
    {
        if (pos.x >= 0 && pos.x < size && pos.y > 0 && pos.y < size)
        {
            return true;
        }
        return false;
    }

    public static bool CanPath(Vector2Int pos)
    {
        if (InGrid(pos) && tiles[pos.x, pos.y] != null && tiles[pos.x, pos.y].type != Tile.Type.water && tiles[pos.x, pos.y].structure == null)
        {
            return true;
        }
        return false;
    }

    public static Tile GetTile(Vector2Int pos)
    {
        if (tiles == null)
        {
            return null;
        }
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

    public static Interaction TargetFromIndex(int index)
    {
        List<Interaction> interactable = GetAllInteractable();

        if (index < interactable.Count)
        {
            return interactable[index];
        }
        else
        {
            return null;
        }
    }

    public static void Reset()
    {
        tiles = null;
        selectedTiles = new List<Tile>();
    }
}
