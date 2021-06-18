using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Tile[,] tiles;
    public Tile selected = null;
    public int mapSize = 25;
    public GameObject grassTile, waterTile, sandTile, dGrassTile;

    public LayerMask tileMask;
    public float cleanRadius = 5;
    public Vector2Int startBase;
    public GameObject[] walls = new GameObject[7];
    public GameObject treePrefab, stonePrefab;
    public int treeScale = 10, stoneScale = 10;
    public List<Interaction> trees = new List<Interaction>();
    public List<Interaction> stones = new List<Interaction>();

 

    public void Generate(Vector2Int basePos)
    {
        tiles = new Tile[mapSize, mapSize];

        Random.seed = System.DateTime.Now.Millisecond;
        Vector2 noiseStart = new Vector2(Random.Range(0, 10000), Random.Range(0, 10000));
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                Vector2 pos = new Vector2(x, y);
                GameObject tile;
    
                float noise = Mathf.PerlinNoise((noiseStart.x + x) / (mapSize / 10), (noiseStart.y + y) / (mapSize / 10));
                if (noise < .25)
                {
                    tile = Instantiate(waterTile, pos, Quaternion.identity);
                }
                else if (noise >= .25 && noise < .35)
                {
                    tile = Instantiate(sandTile, pos, Quaternion.identity);
                }
                else if (noise >= .35 && noise < .75)
                {
                    tile = Instantiate(grassTile, pos, Quaternion.identity);
                }
                else
                {
                    tile = Instantiate(dGrassTile, pos, Quaternion.identity);
                }

                if (tile != null)
                {
                    tiles[x, y] = tile.GetComponent<Tile>();
                    tiles[x, y].Setup();
                }
            }
        }

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                tiles[x, y].UpdateSprite(this, x, y);
            }
        }

        // Place trees
        int numTrees = treeScale * (mapSize / 10);
        for (int i = 0; i < numTrees; i++)
        {
            bool treePlaced = false;
            while (!treePlaced)
            {
                Vector2Int treePos = new Vector2Int((int)(Random.Range(0, mapSize)), (int)(Random.Range(0, mapSize)));
                if (tiles[treePos.x, treePos.y].structure == null && 
                    (tiles[treePos.x, treePos.y].type == Tile.Type.grass || tiles[treePos.x, treePos.y].type == Tile.Type.darkGrass))
                {
                    tiles[treePos.x, treePos.y].structure = Instantiate(treePrefab, tiles[treePos.x, treePos.y].transform.position, Quaternion.identity);
                    trees.Add(tiles[treePos.x, treePos.y].structure.GetComponent<Interaction>());
                    treePlaced = true;
                }
            }
        }

        int numStones = stoneScale * (mapSize / 10);
        for (int i = 0; i < numStones; i++)
        {
            bool stonePlaced = false;
            while (!stonePlaced)
            {
                Vector2Int stonePos = new Vector2Int((int)(Random.Range(0, mapSize)), (int)(Random.Range(0, mapSize)));
                if (tiles[stonePos.x, stonePos.y].structure == null && tiles[stonePos.x, stonePos.y].type != Tile.Type.water)
                {
                    tiles[stonePos.x, stonePos.y].structure = Instantiate(stonePrefab, tiles[stonePos.x, stonePos.y].transform.position, Quaternion.identity);
                    stones.Add(tiles[stonePos.x, stonePos.y].structure.GetComponent<Interaction>());
                    stonePlaced = true;
                }
            }
        }

        CreatureController.Instance.SpawnCreatures(mapSize);
    }

    public bool IsSelected(Collider2D tile)
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

    public void SelectTile(Collider2D tile)
    {
        Vector2Int arrayPos = new Vector2Int((int)(tile.transform.position.x), (int)(tile.transform.position.y));
        if (selected != null)
        {
            selected.Deselect();
            selected = null;
        }

        if (tiles[arrayPos.x, arrayPos.y].type != Tile.Type.water)
        {
            selected = tiles[arrayPos.x, arrayPos.y];
            selected.Select();
        }
    }

    public void DeselectTile()
    {
        if (selected != null)
        {
            selected.Deselect();
            selected = null;
        }
    }
}
