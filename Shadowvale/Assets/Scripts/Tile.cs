using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Tile : MonoBehaviour
{
    public enum Type
    {
        water = 0,
        sand = 1,
        grass = 2,
        darkGrass = 3
    }
    public Type type;
    public Interaction structure;

    public Sprite[] sprites = new Sprite[3];
    public Color corruptedColour;
    private Color highlightColour = Color.red, baseColour, currentColour;
    public SpriteRenderer rend;
    public float corruptionVal = 0;
    public int corruptionMulti = 0;
    float corruptionSpeed = 50f, purifySpeed = 50;
    private bool selected;

    private List<PurifyPillar> pillars = new List<PurifyPillar>();
    public bool isProtected = false;

    public Decoration[,] decor = new Decoration[2, 2];
    public List<GameObject> decorPrefabs = new List<GameObject>();
    public Vector2Int pos;
    public TileCover cover;
    // Generate setup
    public virtual void Setup(int x, int y)
    {
        rend = GetComponent<SpriteRenderer>();
        baseColour = rend.color;
        currentColour = baseColour;
        pos = new Vector2Int(x, y);
    }

    // Load setup
    public virtual void Setup(float corruption, int multi, Vector2Int _pos)
    {
        pos = _pos;
        rend = GetComponent<SpriteRenderer>();
        corruptionVal = corruption;
        baseColour = rend.color;
        currentColour = Color.Lerp(baseColour, corruptedColour, corruptionVal / 100);
        rend.color = currentColour;
        for (int i = 0; i < multi; i++)
        {
            StartCoroutine(CorruptRoutine(pos));
        }
    }
    public void StartSpreading()
    {
        if (corruptionVal >= 100)
        {
            CorruptNeighbours(new Vector2Int((int)transform.position.x, (int)transform.position.y));
        }
    }

    public void Select(bool canBuild)
    {
        selected = true;
        if (canBuild)
        {
            rend.color = Color.green;
        }
        else
        {
            rend.color = Color.red;
        }
    }

    public void Deselect()
    {
        selected = false;
        rend.color = currentColour;
    }
    public Vector2Int GridPos()
    {
        return new Vector2Int((int)transform.position.x, (int)transform.position.y);
    }

    public bool Corrupt(Vector2Int pos)
    {
        if (corruptionVal < 100)
        {
            StartCoroutine(CorruptRoutine(pos));
            return true;
        }
        return false;
    }

    IEnumerator CorruptRoutine(Vector2Int pos)
    {
        corruptionMulti++;
        Resource resource = null;
        if (structure != null && structure is Resource)
        {
            resource = structure.GetComponent<Resource>();
        }
        while (corruptionVal < 100)
        {
            corruptionVal += corruptionSpeed * Time.deltaTime;

            float amount = corruptionVal / 100;
            currentColour = Color.Lerp(baseColour, corruptedColour, amount);
            if (!selected)
            {
                rend.color = currentColour;
                if (resource != null)
                {
                    resource.ChangeColour(amount);
                }
                if (cover.covered)
                {
                    cover.ChangeColour(amount);
                }
            }
            yield return null;
        }
        CorruptNeighbours(pos);
        if (type != Type.water)
        {
            Spawner.Instance.AddCorruptedTile(this);
        }
        StopAllCoroutines();
        corruptionMulti = 0;
    }

    void CorruptNeighbours(Vector2Int pos)
    {
        Vector2Int[] neighbours = Params.Get4Neighbours(pos);
        for (int i = 0; i < 4; i++)
        {
            if (Grid.InGrid(neighbours[i]))
            {
                Tile neighbour = Grid.GetTile(neighbours[i]);
                if (neighbour != null && !neighbour.isProtected)
                {
                    neighbour.Corrupt(neighbours[i]);
                }
            }
        }
    }

    public void Purify(PurifyPillar pillar)
    {
        isProtected = true;
        pillars.Add(pillar);
        StopAllCoroutines();
        StartCoroutine(PurifyRoutine());
    }

    IEnumerator PurifyRoutine()
    {
        Resource resource = null;
        if (structure != null && structure is Resource)
        {
            resource = structure.GetComponent<Resource>();
        }
        while (corruptionVal > 0)
        {
            corruptionVal -= Time.deltaTime * purifySpeed;
            float amount = corruptionVal / 100;
            currentColour = Color.Lerp(baseColour, corruptedColour, amount);
            if (!selected)
            {
                rend.color = currentColour;

            }
            if (resource != null)
            {
                resource.ChangeColour(amount);
            }
            if (cover.covered)
            {
                cover.ChangeColour(amount);
            }
            yield return null;
        }
        Spawner.Instance.RemoveCorruptedTile(this);
        StopAllCoroutines();
    }

    public void RemovePillar(PurifyPillar pillar)
    {
        pillars.Remove(pillar);
        if (pillars.Count == 0)
        {
            isProtected = false;
            if (this != null)
            {
                StopAllCoroutines();
            }
        }

        // check if adjascent to corrupted tile, if so start corrupting
        Vector2Int pos = new Vector2Int((int)transform.position.x, (int)transform.position.y);

        Vector2Int[] neighbours = Params.Get4Neighbours(pos);

        for (int i = 0; i < neighbours.Length; i++)
        {
            if (Grid.InGrid(neighbours[i]) && Grid.GetTile(neighbours[i]).corruptionVal >= 100)
            {
                Corrupt(pos);
            }
        }
    }

    public void Save(TileData data)
    {
        data.type = (int)type;
        data.corruption = corruptionVal;
        data.multi = corruptionMulti;
        data.x = (int)transform.position.x;
        data.y = (int)transform.position.y;

        data.decor1 = (decor[0, 0] == null) ? -1 : decor[0, 0].num;
        data.decor2 = (decor[1, 0] == null) ? -1 : decor[1, 0].num;
        data.decor3 = (decor[0, 1] == null) ? -1 : decor[0, 1].num;
        data.decor4 = (decor[1, 1] == null) ? -1 : decor[1, 1].num;
    }

    public void Load(TileData data)
    {
        Grid.tiles[data.x, data.y] = this;
        Setup(data.corruption, data.multi, new Vector2Int(data.x, data.y));
        if (data.corruption >= 100 && type != Type.water)
        {
            Spawner.Instance.corruptedTiles.Add(Grid.tiles[data.x, data.y]);
        }

        if (type != Type.water)
        {
            if (data.decor1 >= 0)
            {
                SpawnDecor(data.decor1, new Vector2Int(0, 0));
            }
            if (data.decor2 >= 0)
            {
                SpawnDecor(data.decor2, new Vector2Int(1, 0));
            }
            if (data.decor3 >= 0)
            {
                SpawnDecor(data.decor3, new Vector2Int(0, 1));
            }
            if (data.decor4 >= 0)
            {
                SpawnDecor(data.decor4, new Vector2Int(1, 1));
            }
        }
    }

    public void SpawnDecor(int num, Vector2Int pos)
    {
        GameObject decorObj = Instantiate(decorPrefabs[num], new Vector3((pos.x == 0) ? transform.position.x : transform.position.x + 0.5f, (pos.y == 0) ? transform.position.y : transform.position.y + 0.5f, 0), Quaternion.identity);
        decor[pos.x, pos.y] = decorObj.GetComponent<Decoration>();
        decor[pos.x, pos.y].num = num;
    }
}
