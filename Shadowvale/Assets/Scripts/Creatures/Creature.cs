using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : Interaction
{
    public enum Type
    {
        rabbit 
    }
    public Type type;
    public float wanderRange = 10, speed = 10;
    public int maxHealth = 3, health = 0, food = 5;
    public List<Vector2Int> path = new List<Vector2Int>();
    public Vector3 startPos;
    public Vector2Int currentPos, targetPos;
    protected Animator anim;
    protected SpriteRenderer rend;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();

        health = maxHealth;

        startPos = transform.position;
        currentPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        targetPos = RandomTargetPos();
        FindPath();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void Move()
    {
        // Move towards position and keep distance from other followers in squad
        if (path.Count > 0)
        {
            Vector3 pathPos = new Vector3(path[0].x, path[0].y, 0);
            transform.position = Vector2.MoveTowards(transform.position, pathPos, speed * Time.deltaTime);
            SetAnimDir();

            if (transform.position == pathPos)
            {
                currentPos = path[0];
                path.RemoveAt(0);
            }
        }
        else
        {
            targetPos = RandomTargetPos();
            FindPath();
        }
    }

    public Vector2Int RandomTargetPos()
    {
        bool selected = false;
        Vector2Int pos = new Vector2Int((int)Random.Range(0, Grid.size), (int)Random.Range(0, Grid.size));
        while (!selected)
        {
            if (Grid.CanPath(pos))
            {
                return pos;
            }
            else
            {
                pos = new Vector2Int((int)Random.Range(0, Grid.size), (int)Random.Range(0, Grid.size));
            }
        }
        return new Vector2Int((int)Random.Range(0, Grid.size), (int)Random.Range(0, Grid.size));

    }

    public void FindPath()
    {
        Pathfinding.FindPath(ref path, currentPos, targetPos);
    }


    public bool Hit(int damage)
    {
        health -= damage;
        StartCoroutine(HitRoutine());

        if (health <= 0)
        {
            return true;
        }
        return false;
    }

    IEnumerator HitRoutine()
    {
        rend.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.color = Color.white;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void GatherFood(Inventory inv)
    {
        inv.resources[(int)Resource.Type.food] += food;
    }

    void SetAnimDir()
    {
        float xDiff = Mathf.Abs(transform.position.x - path[0].x), yDiff = Mathf.Abs(transform.position.y - path[0].y);
        if (xDiff > yDiff)
        {
            xDiff = transform.position.x - targetPos.x;
            if (xDiff < 0)
            {
                anim.SetInteger("Direction", 1);
            }
            else if (xDiff > 0)
            {
                anim.SetInteger("Direction", 3);
            }
        }
        else if (yDiff > xDiff)
        {
            yDiff = transform.position.y - targetPos.y;
            if (yDiff < 0)
            {
                anim.SetInteger("Direction", 0);
            }
            else if (yDiff > 0)
            {
                anim.SetInteger("Direction", 2);
            }
        }
    }
}
