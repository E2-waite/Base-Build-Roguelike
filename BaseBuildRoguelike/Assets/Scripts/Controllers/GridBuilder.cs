using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilder : MonoSingleton<GridBuilder>
{
    public GameObject grassTile, waterTile, sandTile, dGrassTile;
    public GameObject treePrefab, stonePrefab;
    public int mapSize = 25, noise = 250;
    public int treeScale = 10, stoneScale = 10;

    public void Generate()
    {
        Grid.Init(mapSize, noise);
        Random.seed = System.DateTime.Now.Millisecond;
        Vector2 noiseStart = new Vector2(Random.Range(0, 10000), Random.Range(0, 10000));
        for (int y = 0; y < Grid.size; y++)
        {
            for (int x = 0; x < Grid.size; x++)
            {
                Vector2 pos = new Vector2(x, y);
                GameObject tile;

                float noise = Mathf.PerlinNoise((noiseStart.x + x) / (Grid.noise / 10), (noiseStart.y + y) / (Grid.noise / 10));
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
                    Grid.tiles[x, y] = tile.GetComponent<Tile>();
                    Grid.tiles[x, y].Setup();
                }
            }
        }

        for (int y = 0; y < Grid.size; y++)
        {
            for (int x = 0; x < Grid.size; x++)
            {
                Grid.tiles[x, y].UpdateSprite(x, y);
            }
        }

        // Place trees
        int numTrees = treeScale * (Grid.size / 10);
        for (int i = 0; i < numTrees; i++)
        {
            bool treePlaced = false;
            while (!treePlaced)
            {
                Vector2Int treePos = new Vector2Int((int)(Random.Range(0, mapSize)), (int)(Random.Range(0, mapSize)));
                if (Grid.tiles[treePos.x, treePos.y].structure == null &&
                    (Grid.tiles[treePos.x, treePos.y].type == Tile.Type.grass || Grid.tiles[treePos.x, treePos.y].type == Tile.Type.darkGrass))
                {
                    GameObject obj = Instantiate(treePrefab, Grid.tiles[treePos.x, treePos.y].transform.position, Quaternion.identity);
                    Grid.tiles[treePos.x, treePos.y].structure = obj.GetComponent<Interaction>();
                    Resources.trees.Add(Grid.tiles[treePos.x, treePos.y].structure);
                    Resources.allResources.Add(Grid.tiles[treePos.x, treePos.y].structure);
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
                if (Grid.tiles[stonePos.x, stonePos.y].structure == null && Grid.tiles[stonePos.x, stonePos.y].type != Tile.Type.water)
                {
                    GameObject obj = Instantiate(stonePrefab, Grid.tiles[stonePos.x, stonePos.y].transform.position, Quaternion.identity);
                    Grid.tiles[stonePos.x, stonePos.y].structure = obj.GetComponent<Interaction>();
                    Resources.stones.Add(Grid.tiles[stonePos.x, stonePos.y].structure);
                    Resources.allResources.Add(Grid.tiles[stonePos.x, stonePos.y].structure);
                    stonePlaced = true;
                }
            }
        }

        Spawner.Instance.SpawnCreatures(mapSize);

        Vector2Int corruptPos = new Vector2Int(Random.Range(0, mapSize), Random.Range(0, mapSize));
        Debug.Log(corruptPos.ToString() + " Started Corruption");
        Grid.tiles[corruptPos.x, corruptPos.y].Corrupt(corruptPos);

        Pathfinding.UpdateNodeGrid();
    }
}
