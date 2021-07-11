using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Interaction
{
    [Header("Enemy Settings")]
    public int maxHealth = 3, health, hitDamage = 1;
    public float speed = 2, targetDist = 1f, targetRange = 15;
    public Interaction target;
    public LayerMask buildingMask;
    public Squad squad, targetSquad;
    protected SpriteRenderer rend;
    protected Animator anim;
    public GameObject squadPrefab;
    private void Start()
    {
        target = GameController.Instance.homeBuilding;
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        health = maxHealth;
    }

    public void Move(Vector3 position)
    {
        transform.position = Vector2.MoveTowards(transform.position, position, speed * Time.deltaTime);
        float yDiff = position.y - transform.position.y, xDiff = position.x - transform.position.x;
        anim.SetInteger("Direction", Mathf.RoundToInt(yDiff));
        if (squad != null)
        {
            foreach (Enemy enemies in squad.members)
            {
                if (enemies != this && enemies != null)
                {
                    float dist = Vector3.Distance(transform.position, enemies.transform.position);
                    if (dist <= 0.5)
                    {
                        Vector3 diffVec = transform.position - enemies.transform.position;
                        transform.position = Vector2.MoveTowards(transform.position, transform.position + diffVec, speed * Time.deltaTime);
                    }
                }
            }
        }

        // Damage building in path
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(xDiff, yDiff), .2f, buildingMask);

        //if (hit.collider != null && !(target is Follower) && target.gameObject != hit.collider.gameObject)
        //{
        //    target = hit.collider.GetComponent<Interaction>();
        //}
    }

    public void JoinSquad(Enemy enemy)
    {
        // Join/Merge squads
        if (enemy.squad == null && squad == null)
        {
            // No squad - create one
            GameObject newSquad = Instantiate(squadPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            squad = newSquad.GetComponent<Squad>();
            squad.Setup(this, enemy);
            return;
        }
        else if (enemy.squad != null && squad == null)
        {
            enemy.squad.AddMember(this);
            return;
        }
        else if (enemy.squad == null && squad != null)
        {
            squad.AddMember(enemy);
            return;
        }
        else
        {
            squad.Combine(enemy.squad);
        }
    }

    // GET HIT
    public bool Hit(int damage, Follower attacker)
    {
        Debug.Log("HIT");
        health -= damage;

        if (!(target is Follower))
        {
            // Update target if current target is not an enemy (stops switching target while in combat)
            target = attacker;
            if (attacker.squad != null)
            {
                if (squad == null)
                {
                    targetSquad = attacker.squad;
                }
            }
            squad.SetTarget(attacker);
        }

        StartCoroutine(HitRoutine());

        if (health <= 0)
        {
            return true;
        }
        return false;
    }

    public void UpdateTarget(Interaction newTarget)
    {
        if (!(target is Follower))
        {
            target = newTarget;
        }
    }

    IEnumerator HitRoutine()
    {
        rend.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.color = Color.white;
        if (health <= 0)
        {
            EnemyController.Instance.Remove(this);
            if (squad != null)
            {
                squad.RemoveMember(this);
            }
            Destroy(gameObject);
        }
    }

}
