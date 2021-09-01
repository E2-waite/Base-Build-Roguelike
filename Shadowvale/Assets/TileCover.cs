using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCover : MonoBehaviour
{
    public bool cover = true, covered = false;
    public SpriteRenderer rend;
    public Sprite[] straightSprites = new Sprite[4], innerSprites = new Sprite[4], outerSprites = new Sprite[4];
    public Sprite filledSprite;
    Color startColour;
    public Color corruptColour;

    private void Start()
    {
        startColour = rend.color;
    }
    public void ChangeColour(float val)
    {
        rend.color = Color.Lerp(startColour, corruptColour, val);
    }
    public bool CoverTile(Tile tile)
    {
        if (cover)
        {
            Tile[] neighbours = new Tile[8];
            bool[] dirs = new bool[8];
            Vector2Int[] positions = Params.Get8Neighbours(tile.pos);
            bool bordering = false;
            for (int i = 0; i < 8; i++)
            {
                if (Grid.InGrid(positions[i]))
                {
                    neighbours[i] = Grid.tiles[positions[i].x, positions[i].y];
                    if (neighbours[i] != null && tile.type == neighbours[i].type - 1)
                    {
                        // If neighbouring tile is one above the current tile, is bordering
                        dirs[i] = true;
                        bordering = true;
                    }
                }
            }

            if (!bordering)
            {
                return false;
            }

            if (dirs[0] && !dirs[2] && !dirs[4] && !dirs[6])
            {
                rend.sprite = straightSprites[0];
            }
            else if (!dirs[0] && !dirs[2] && !dirs[4] && dirs[6])
            {
                rend.sprite = straightSprites[1];
            }
            else if (!dirs[0] && !dirs[2] && dirs[4] && !dirs[6])
            {
                rend.sprite = straightSprites[2];
            }
            else if (!dirs[0] && dirs[2] && !dirs[4] && !dirs[6])
            {
                rend.sprite = straightSprites[3];
            }
            else
            {
                if (dirs[0] && dirs[6] && dirs[7] && !dirs[2] && !dirs[4])
                {
                    rend.sprite = innerSprites[0];
                }
                else if (dirs[0] && dirs[1] && dirs[2] && !dirs[4] && !dirs[6])
                {
                    rend.sprite = innerSprites[1];
                }
                else if (dirs[2] && dirs[3] && dirs[4] && !dirs[0] && !dirs[6])
                {
                    rend.sprite = innerSprites[2];
                }
                else if (dirs[4] && dirs[5] && dirs[6] && !dirs[0] && !dirs[2])
                {
                    rend.sprite = innerSprites[3];
                }
                else
                {
                    if (dirs[7] && !dirs[0] && !dirs[2] && !dirs[4] && !dirs[6])
                    {
                        rend.sprite = outerSprites[0];
                    }
                    else if (dirs[1] && !dirs[0] && !dirs[2] && !dirs[4] && !dirs[6])
                    {
                        rend.sprite = outerSprites[1];
                    }
                    else if (dirs[3] && !dirs[0] && !dirs[2] && !dirs[4] && !dirs[6])
                    {
                        rend.sprite = outerSprites[2];
                    }
                    else if (dirs[5] && !dirs[0] && !dirs[2] && !dirs[4] && !dirs[6])
                    {
                        rend.sprite = outerSprites[3];
                    }
                    else
                    {
                        rend.sprite = filledSprite;
                        Debug.Log("FILLLED");
                    }
                }
            }
            covered = true;
            return true;
        }
        return false;
    }
}
