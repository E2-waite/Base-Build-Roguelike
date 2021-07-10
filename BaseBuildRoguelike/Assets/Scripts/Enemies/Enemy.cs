using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Interaction
{
    [Header("Enemy Settings")]
    public int maxHealth = 3, health, hitDamage = 1;
    public float speed = 2, targetDist = 0.25f;
    public Interaction target;
    public LayerMask buildingMask;
    public Squad squad, targetSquad;
    protected SpriteRenderer rend;
    protected Animator anim;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        health = maxHealth;
        anim = GetComponent<Animator>();
        //target = GameController.Instance.homeBuilding;
    }
    public void Move(Vector3 position)
    {
        transform.position = Vector2.MoveTowards(transform.position, position, speed * Time.deltaTime);
        float yDiff = position.y - transform.position.y, xDiff = position.x - transform.position.x;
        anim.SetInteger("Direction", Mathf.RoundToInt(yDiff));
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(xDiff, yDiff), .2f, buildingMask);

        if (hit.collider != null && !(target is Follower) && target.gameObject != hit.collider.gameObject)
        {
            target = hit.collider.GetComponent<Interaction>();
        }
    }

    // GET HIT
    public bool Hit(int damage, Follower attacker)
    {
        Debug.Log("HIT");
        health -= damage;
        target = attacker;
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
            EnemyController.Instance.Remove(this);
            Destroy(gameObject);
        }
    }

}
