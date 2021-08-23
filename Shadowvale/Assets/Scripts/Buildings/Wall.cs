using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : Building
{
    public Sprite[] wallSprites;

    public override void Setup()
    {
        if (rend == null)
        {
            rend = GetComponent<SpriteRenderer>();
        }
        Vector2Int pos = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        Buildings.walls[pos.x, pos.y] = this;
        CheckDirs(pos);
        UpdateNeighbours(pos);
    }

    public override void Destroy()
    {
        Vector2Int pos = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        Buildings.walls[pos.x, pos.y] = null;
        base.Destroy();
    }

    public void UpdateSprite(Vector2Int pos)
    {
        CheckDirs(pos);
    }

    void UpdateNeighbours(Vector2Int pos)
    {
        if (CheckWall(new Vector2Int(pos.x, pos.y + 1)))
        {
            Buildings.walls[pos.x, pos.y + 1].UpdateSprite(new Vector2Int(pos.x, pos.y + 1));
        }
        if (CheckWall(new Vector2Int(pos.x + 1, pos.y)))
        {
            Buildings.walls[pos.x + 1, pos.y].UpdateSprite(new Vector2Int(pos.x + 1, pos.y));
        }
        if (CheckWall(new Vector2Int(pos.x, pos.y - 1)))
        {
            Buildings.walls[pos.x, pos.y - 1].UpdateSprite(new Vector2Int(pos.x, pos.y - 1));
        }
        if (CheckWall(new Vector2Int(pos.x - 1, pos.y)))
        {
            Buildings.walls[pos.x - 1, pos.y].UpdateSprite(new Vector2Int(pos.x - 1, pos.y));
        }
    }

    void CheckDirs(Vector2Int pos)
    {
        bool[] dirs = new bool[4];
        if (CheckWall(new Vector2Int(pos.x, pos.y + 1)))
        {
            dirs[0] = true;
        }
        if (CheckWall(new Vector2Int(pos.x + 1, pos.y)))
        {
            dirs[1] = true;
        }
        if (CheckWall(new Vector2Int(pos.x, pos.y - 1)))
        {
            dirs[2] = true;
        }
        if (CheckWall(new Vector2Int(pos.x - 1, pos.y)))
        {
            dirs[3] = true;
        }

        SetWallSprites(dirs);
    }

    bool CheckWall(Vector2Int pos)
    {
        if (Buildings.walls[pos.x, pos.y] != null)
        {
            return true;
        }
        return false;
    }

    void SetWallSprites(bool[] dirs)
    {
        if (!dirs[0] && !dirs[1] && !dirs[2] && !dirs[3])
        {
            rend.sprite = wallSprites[0];
        }
        else if (!dirs[0] && dirs[1] && !dirs[2] && dirs[3])
        {
            rend.sprite = wallSprites[1];
        }
        else if(dirs[0] && !dirs[1] && dirs[2] && !dirs[3])
        {
            rend.sprite = wallSprites[2];
        }
        else if (!dirs[0] && dirs[1] && !dirs[2] && !dirs[3])
        {
            rend.sprite = wallSprites[4];
        }
        else if (!dirs[0] && !dirs[1] && !dirs[2] && dirs[3])
        {
            rend.sprite = wallSprites[5];
        }
        else if (!dirs[0] && !dirs[1] && dirs[2] && !dirs[3])
        {
            rend.sprite = wallSprites[6];
        }
        else if (dirs[0] && !dirs[1] && !dirs[2] && !dirs[3])
        {
            rend.sprite = wallSprites[7];
        }
        else if (!dirs[0] && dirs[1] && dirs[2] && !dirs[3])
        {
            rend.sprite = wallSprites[8];
        }
        else if (!dirs[0] && !dirs[1] && dirs[2] && dirs[3])
        {
            rend.sprite = wallSprites[9];
        }
        else if (dirs[0] && dirs[1] && !dirs[2] && !dirs[3])
        {
            rend.sprite = wallSprites[10];
        }
        else if (dirs[0] && !dirs[1] && !dirs[2] && dirs[3])
        {
            rend.sprite = wallSprites[11];
        }
        else
        {
            rend.sprite = wallSprites[3];
        }
    }
}
