using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cooldown
{
    public float max, current = 0;
    public Cooldown(float time)
    {
        max = time;
        current = max;
    }

    public bool Tick()
    {
        if (current > 0)
        {
            current -= Time.deltaTime;
            return false;
        }
        return true;
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
    public Interaction interact = null;
    public Squad squad = null;
    public Vector2Int lastPos;
    public bool staticObject = false;

    public Target()
    {

    }

    public Target(Interaction target)
    {
        interact = target;
        if (interact != null)
        {
            lastPos = Position2D();
            staticObject = target.staticObject;

            if (!staticObject)
            {
                if (target is Follower)
                {
                    squad = (target as Follower).squad;
                }
                else if (target is Enemy)
                {
                    squad = (target as Enemy).squad;
                }
            }
        }
    }

    public Vector3 Position()
    {
        return interact.transform.position;
    }

    public Vector2Int Position2D()
    {
        return new Vector2Int(Mathf.RoundToInt(interact.transform.position.x), Mathf.RoundToInt(interact.transform.position.y));
    }

    public Vector2Int LastPos()
    {
        Vector2Int pos = lastPos;
        lastPos = Position2D();
        return pos;
    }

    public bool UpdatePath()
    {
        if (staticObject || Position2D() == LastPos())
        {
            return false;
        }
        return true;
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