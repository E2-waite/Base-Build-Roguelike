using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public GameObject structure;

    public Sprite[] sprites = new Sprite[3];
    public Color corruptedColour;
    Color highlightColour = Color.red, baseColour;
    public SpriteRenderer rend;

    public float corruptionVal = 0;

    public virtual void Setup()
    {
        rend = GetComponent<SpriteRenderer>();
        baseColour = rend.color;
    }

    public void Select()
    {
        rend.color = highlightColour;
    }

    public void Deselect()
    {
        rend.color = baseColour;
    }

    public void UpdateSprite(Grid grid, int x, int y)
    {
        bool higherTile = false;
        if (y > 0 && grid.tiles[x, y - 1].type < type)
        {
            higherTile = true;
        }

        bool lowerTile = false;
        if (y < grid.tiles.GetLength(1) - 1 && grid.tiles[x, y + 1].type > type)
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

    IEnumerator CorruptRoutine()
    {
        while (corruptionVal < 100)
        {
            corruptionVal += Time.deltaTime;
            yield return null;
        }
    }
}
