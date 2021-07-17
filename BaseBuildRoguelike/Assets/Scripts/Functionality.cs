using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooldown
{
    private float max, current = 0;
    public Cooldown(float time)
    {
        max = time;
        current = max;
    }

    public void Tick()
    {
        if (current > 0)
        {
            current -= Time.deltaTime;
        }
    }

    public void Reset()
    {
        current = max;
    }

    public bool Complete()
    {
        if (current <= 0)
        {
            return true;
        }
        return false;
    }
    
}

public class Target
{
    public Interaction interact;
    public Squad squad;
    public Vector2Int lastPos;
    public bool staticObject = false;

    public Target(Interaction target, Vector2Int pos, bool isStatic, Squad squad = null)
    {
        interact = target;
        lastPos = pos;
        staticObject = isStatic;
    }
}

public class Params
{
    /// <summary>Return the 4 adjacent neighbour positions</summary>
    public static Vector2Int[] Get4Neighbours(Vector2Int pos)
    {
        return new Vector2Int[4] { new Vector2Int(pos.x, pos.y + 1), new Vector2Int(pos.x + 1, pos.y), new Vector2Int(pos.x, pos.y - 1), new Vector2Int(pos.x - 1, pos.y) };
    }

    /// <summary>Return the 8 surrounding neighbour positions</summary>
    public static Vector2Int[] Get8Neighbours(Vector2Int pos)
    {
        return new Vector2Int[8] { new Vector2Int(pos.x, pos.y + 1), new Vector2Int(pos.x + 1, pos.y + 1), new Vector2Int(pos.x + 1, pos.y), new Vector2Int(pos.x + 1, pos.y - 1), 
            new Vector2Int(pos.x, pos.y - 1), new Vector2Int(pos.x - 1, pos.y - 1), new Vector2Int(pos.x - 1, pos.y), new Vector2Int(pos.x - 1, pos.y + 1) };
    }
}