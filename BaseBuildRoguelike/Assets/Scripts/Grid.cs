using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Tile[,] tiles;
    public Tile selected = null;
    public int mapSize = 25;
    public float tileSize = 1;
    public GameObject tile;
    public Color healthyColour, deadColour;

    public LayerMask tileMask;
    public float cleanRadius = 5;
    public Vector2Int startBase;
    public GameObject[] walls = new GameObject[7];
    public GameObject treePrefab, stonePrefab;
    public int treeScale = 10, stoneScale = 10;
    public List<Interaction> trees = new List<Interaction>();
    public List<Interaction> stones = new List<Interaction>();

    public class Tile
    {
        public bool corrupted;
        public GameObject tile;
        Vector2 position;
        public GameObject structure;
        Color selectColour = Color.red, baseColour;
        

        public Tile(GameObject tileObj, Vector2 tilePos, GameObject tileStructure, float size, Color colour, bool dead)
        {
            tile = tileObj;
            position = tilePos;
            structure = tileStructure;
            baseColour = colour;
            tile.transform.localScale = new Vector2(size, size);
            corrupted = dead;
            if (tileStructure != null)
            {
                tileStructure.transform.localScale = new Vector2(size, size);
            }

            SetColour(baseColour, true);
        }

        public void SetColour(Color colour, bool overWrite)
        {
            tile.GetComponent<SpriteRenderer>().color = colour;
            if (overWrite)
            {
                baseColour = colour;
            }
        }

        public void Select()
        {
            SetColour(selectColour, false);
        }

        public void Deselect()
        {
            SetColour(baseColour, false);
        }
    }

    void Start()
    {

    }

    public void Generate(Vector2Int basePos)
    {
        tiles = new Tile[mapSize, mapSize];
        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                Vector2 pos = new Vector2(x * tileSize, y * tileSize);
                tiles[x, y] = new Tile(Instantiate(tile, pos, Quaternion.identity), pos, null, tileSize, deadColour, true);
            }
        }

        Collider2D[] cols = Physics2D.OverlapCircleAll(basePos, cleanRadius, tileMask);

        Debug.Log(basePos.ToString());

        foreach (Collider2D col in cols)
        {
            tiles[(int)(col.transform.position.x / tileSize), (int)(col.transform.position.y / tileSize)].SetColour(healthyColour, true);
            tiles[(int)(col.transform.position.x / tileSize), (int)(col.transform.position.y / tileSize)].corrupted = false;
        }

        // Place trees
        int numTrees = treeScale * (mapSize / 10);
        for (int i = 0; i < numTrees; i++)
        {
            bool treePlaced = false;
            while (!treePlaced)
            {
                Vector2Int treePos = new Vector2Int((int)(Random.Range(0, mapSize * tileSize)), (int)(Random.Range(0, mapSize * tileSize)));
                if (tiles[treePos.x, treePos.y].structure == null)
                {
                    tiles[treePos.x, treePos.y].structure = Instantiate(treePrefab, tiles[treePos.x, treePos.y].tile.transform.position, Quaternion.identity);
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
                Vector2Int stonePos = new Vector2Int((int)(Random.Range(0, mapSize * tileSize)), (int)(Random.Range(0, mapSize * tileSize)));
                if (tiles[stonePos.x, stonePos.y].structure == null)
                {
                    tiles[stonePos.x, stonePos.y].structure = Instantiate(stonePrefab, tiles[stonePos.x, stonePos.y].tile.transform.position, Quaternion.identity);
                    stones.Add(tiles[stonePos.x, stonePos.y].structure.GetComponent<Interaction>());
                    stonePlaced = true;
                }
            }
        }
    }

    public bool IsSelected(Collider2D tile)
    {
        Vector2Int arrayPos = new Vector2Int((int)(tile.transform.position.x / tileSize), (int)(tile.transform.position.y / tileSize));
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
        Vector2Int arrayPos = new Vector2Int((int)(tile.transform.position.x / tileSize), (int)(tile.transform.position.y / tileSize));
        if (selected != null)
        {
            selected.Deselect();
        }
        selected = tiles[arrayPos.x, arrayPos.y];
        selected.Select();
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
