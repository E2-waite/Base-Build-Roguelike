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
    float corruptionSpeed = 5f, purifySpeed = 50;
    private bool selected;

    private List<PurifyPillar> pillars = new List<PurifyPillar>();
    public bool isProtected = false;

    // Generate setup
    public virtual void Setup()
    {
        rend = GetComponent<SpriteRenderer>();
        baseColour = rend.color;
        currentColour = baseColour;
    }

    // Load setup
    public virtual void Setup(float corruption)
    {
        rend = GetComponent<SpriteRenderer>();
        corruptionVal = corruption;
        baseColour = rend.color;
        currentColour = Color.Lerp(baseColour, corruptedColour, corruptionVal / 100);
        rend.color = currentColour;

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

    public void UpdateSprite(int x, int y)
    {
        bool higherTile = false;
        if (y > 0 && Grid.tiles[x, y - 1] != null && Grid.tiles[x, y - 1].type < type)
        {
            higherTile = true;
        }

        bool lowerTile = false;
        if (y < Grid.tiles.GetLength(1) - 1 && Grid.tiles[x, y + 1] != null && Grid.tiles[x, y + 1].type > type)
        {
            lowerTile = true;
        }

        if (higherTile && lowerTile)
        {
            rend.sprite = sprites[2];
        }
        else if (higherTile && !lowerTile)
        {
            rend.sprite = sprites[0];
        }
        else if (!higherTile && lowerTile)
        {
            rend.sprite = sprites[1];
        }
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

        while (corruptionVal < 100)
        {
            corruptionVal += corruptionSpeed * Time.deltaTime;

            float amount = corruptionVal / 100;
            currentColour = Color.Lerp(baseColour, corruptedColour, amount);
            if (!selected)
            {
                rend.color = currentColour;
            }
            yield return null;
        }
        CorruptNeighbours(pos);
        Spawner.Instance.AddCorruptedTile(this);
        StopAllCoroutines();
    }

    void CorruptNeighbours(Vector2Int pos)
    {
        Vector2Int[] neighbours = Params.Get4Neighbours(pos);
        for (int i = 0; i < 4; i++)
        {
            if (Grid.InGrid(neighbours[i]))
            {
                Tile neighbour = Grid.GetTile(neighbours[i]);
                if (neighbour.type != Type.water && !neighbour.isProtected)
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
        while (corruptionVal > 0)
        {
            corruptionVal -= Time.deltaTime * purifySpeed;
            float amount = corruptionVal / 100;
            currentColour = Color.Lerp(baseColour, corruptedColour, amount);
            if (!selected)
            {
                rend.color = currentColour;
            }

            //if (structure != null)
            //{
            //    structure.Corrupt(corruptedColour, amount);
            //}
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
}
