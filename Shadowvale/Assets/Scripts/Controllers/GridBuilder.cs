using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuilder : MonoSingleton<GridBuilder>
{
    public GameObject grassTile, waterTile, sandTile, dGrassTile;
    public int mapSize = 25, noise = 250;
    public int treeScale = 10, stoneScale = 10, decorThreshold = 150;

    public void Generate()
    {
        Grid.Init(mapSize, noise);
        Vector2Int centre = new Vector2Int((int)(Grid.size / 2), (int)(Grid.size / 2));
        Random.seed = System.DateTime.Now.Millisecond;
        Vector2 noiseStart = new Vector2(Random.Range(0, 10000), Random.Range(0, 10000));
        for (int y = 0; y < Grid.size; y++)
        {
            for (int x = 0; x < Grid.size; x++)
            {
                Vector2 pos = new Vector2(x, y);
                
                if (Vector2.Distance(pos, centre) <= Grid.size / 2)
                {
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
        }

        bool startSet = false;
        while (!startSet)
        {
            Grid.startPos = new Vector2Int(Random.Range(centre.x - (Grid.size / 5), centre.x + (Grid.size / 5)), Random.Range(centre.y - (Grid.size / 5), centre.y + (Grid.size / 5)));
            Tile baseTile = Grid.tiles[Grid.startPos.x, Grid.startPos.y], followerTile = Grid.tiles[Grid.startPos.x, Grid.startPos.y - 1];
            if (baseTile != null && followerTile != null && baseTile.type != Tile.Type.water && followerTile.type != Tile.Type.water && baseTile.structure == null && followerTile.structure == null)
            {
                startSet = true;
            }
        }

        for (int y = 0; y < Grid.size; y++)
        {
            for (int x = 0; x < Grid.size; x++)
            {
                if (Grid.tiles[x, y] != null)
                {
                    Grid.tiles[x, y].UpdateSprite(x, y);
                }
            }
        }

        // Place trees
        Resources.maxTrees = treeScale * (Grid.size / 10);
        for (int i = 0; i < Resources.maxTrees; i++)
        {
            bool treePlaced = false;
            float rarity = Random.Range(0, 100);
            GameObject prefab = Spawner.Instance.ResourcePrefab(Resource.Type.wood, rarity);
            while (!treePlaced)
            {
                Vector2Int treePos = new Vector2Int((int)(Random.Range(0, mapSize)), (int)(Random.Range(0, mapSize)));
                if (Grid.tiles[treePos.x, treePos.y] != null && Grid.tiles[treePos.x, treePos.y].structure == null &&
                    (Grid.tiles[treePos.x, treePos.y].type == Tile.Type.grass || Grid.tiles[treePos.x, treePos.y].type == Tile.Type.darkGrass))
                {
                    GameObject obj = Instantiate(prefab, Grid.tiles[treePos.x, treePos.y].transform.position, Quaternion.identity);
                    Grid.tiles[treePos.x, treePos.y].structure = obj.GetComponent<Interaction>();
                    Resources.trees.Add(Grid.tiles[treePos.x, treePos.y].structure);
                    Resources.allResources.Add(Grid.tiles[treePos.x, treePos.y].structure);
                    treePlaced = true;
                }
            }
        }

        Resources.maxStones = stoneScale * (mapSize / 10);
        for (int i = 0; i < Resources.maxStones; i++)
        {
            bool stonePlaced = false;
            float rarity = Random.Range(0, 100);
            GameObject prefab = Spawner.Instance.ResourcePrefab(Resource.Type.stone, rarity);
            while (!stonePlaced)
            {
                Vector2Int stonePos = new Vector2Int((int)(Random.Range(0, mapSize)), (int)(Random.Range(0, mapSize)));
                if (Grid.tiles[stonePos.x, stonePos.y] != null && Grid.tiles[stonePos.x, stonePos.y].structure == null && Grid.tiles[stonePos.x, stonePos.y].type != Tile.Type.water)
                {
                    GameObject obj = Instantiate(prefab, Grid.tiles[stonePos.x, stonePos.y].transform.position, Quaternion.identity);
                    Grid.tiles[stonePos.x, stonePos.y].structure = obj.GetComponent<Interaction>();
                    Resources.stones.Add(Grid.tiles[stonePos.x, stonePos.y].structure);
                    Resources.allResources.Add(Grid.tiles[stonePos.x, stonePos.y].structure);
                    stonePlaced = true;
                }
            }
        }

        for (int yi = 0; yi < Grid.size; yi++)
        {
            for (int xi = 0; xi < Grid.size; xi++)
            {
                if (Grid.tiles[xi, yi] != null)
                {
                    Tile tile = Grid.tiles[xi, yi];
                    if (tile.type != Tile.Type.water && tile.structure == null)
                    {
                        for (int yj = 0; yj < 2; yj++)
                        {
                            for (int xj = 0; xj < 2; xj++)
                            {
                                int rand = Random.Range(0, decorThreshold + tile.decorPrefabs.Count + 1);

                                if (rand > decorThreshold)
                                {
                                    tile.SpawnDecor(rand - decorThreshold - 1, new Vector2Int(xj, yj));
                                }
                            }
                        }
                    }
                    else
                    {
                        Debug.Log("Unsuitable");
                    }
                }
            }
        }

        Spawner.Instance.SpawnCreatures();

        //bool corrupted = false;
        //while (!corrupted)
        //{
        //    Vector2Int corruptPos = new Vector2Int(Random.Range(0, mapSize), Random.Range(0, mapSize));
        //    float dist = Vector2Int.Distance(corruptPos, Grid.startPos);
        //    if (Grid.tiles[corruptPos.x, corruptPos.y] != null && Grid.tiles[corruptPos.x, corruptPos.y].type != Tile.Type.water && dist > Grid.size / 4)
        //    {
        //        Debug.Log(corruptPos.ToString() + " Started Corruption");
        //        Grid.tiles[corruptPos.x, corruptPos.y].Corrupt(corruptPos);
        //        corrupted = true;
        //    }
        //}

        Pathfinding.UpdateNodeGrid();
    }
}
