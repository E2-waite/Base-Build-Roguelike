using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCover : MonoBehaviour
{
    public bool cover = true;
    public SpriteRenderer coverRend;
    public Sprite[] straightSprites = new Sprite[4], innerSprites = new Sprite[4], outerSprites = new Sprite[4];
    public Sprite filledSprite;
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
                coverRend.sprite = straightSprites[0];
            }
            else if (!dirs[0] && !dirs[2] && !dirs[4] && dirs[6])
            {
                coverRend.sprite = straightSprites[1];
            }
            else if (!dirs[0] && !dirs[2] && dirs[4] && !dirs[6])
            {
                coverRend.sprite = straightSprites[2];
            }
            else if (!dirs[0] && dirs[2] && !dirs[4] && !dirs[6])
            {
                coverRend.sprite = straightSprites[3];
            }
            else
            {
                if (dirs[0] && dirs[6] && dirs[7] && !dirs[2] && !dirs[4])
                {
                    coverRend.sprite = innerSprites[0];
                }
                else if (dirs[0] && dirs[1] && dirs[2] && !dirs[4] && !dirs[6])
                {
                    coverRend.sprite = innerSprites[1];
                }
                else if (dirs[2] && dirs[3] && dirs[4] && !dirs[0] && !dirs[6])
                {
                    coverRend.sprite = innerSprites[2];
                }
                else if (dirs[4] && dirs[5] && dirs[6] && !dirs[0] && !dirs[2])
                {
                    coverRend.sprite = innerSprites[3];
                }
                else
                {
                    if (dirs[7] && !dirs[0] && !dirs[2] && !dirs[4] && !dirs[6])
                    {
                        coverRend.sprite = outerSprites[0];
                    }
                    else if (dirs[1] && !dirs[0] && !dirs[2] && !dirs[4] && !dirs[6])
                    {
                        coverRend.sprite = outerSprites[1];
                    }
                    else if (dirs[3] && !dirs[0] && !dirs[2] && !dirs[4] && !dirs[6])
                    {
                        coverRend.sprite = outerSprites[2];
                    }
                    else if (dirs[5] && !dirs[0] && !dirs[2] && !dirs[4] && !dirs[6])
                    {
                        coverRend.sprite = outerSprites[3];
                    }
                    else
                    {
                        coverRend.sprite = filledSprite;
                        Debug.Log("FILLLED");
                    }
                }
            }
            return true;
        }
        return false;
    }
}
