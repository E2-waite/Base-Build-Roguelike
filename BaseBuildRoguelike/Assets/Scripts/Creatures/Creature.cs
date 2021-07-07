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
    Vector3 startPos, targetPos;
    protected Animator anim;
    protected SpriteRenderer rend;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        startPos = transform.position;
        targetPos = RandomTargetPos();
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position == targetPos)
        {
            targetPos = RandomTargetPos();
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            SetAnimDir();
        }
    }

    Vector2 RandomTargetPos()
    {
        return new Vector3(Random.Range(startPos.x - wanderRange, startPos.x + wanderRange), Random.Range(startPos.y - wanderRange, startPos.y + wanderRange), 0);
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
        float xDiff = Mathf.Abs(transform.position.x - targetPos.x), yDiff = Mathf.Abs(transform.position.y - targetPos.y);
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
