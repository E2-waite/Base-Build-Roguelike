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
    public List<Vector2Int> path = new List<Vector2Int>();
    protected SpriteRenderer rend;
    protected Animator anim;
    public GameObject squadPrefab;
    private void Start()
    {
        target = Buildings.homeBase;
        if (target != null)
        {
            Pathfinding.FindPath(ref path, transform.position, target.transform.position);
        }
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        health = maxHealth;
        
        StartCoroutine(PathUpdate());
    }

    protected void Move()
    {
        if (path.Count > 0)
        {
            Vector3 pathPos = new Vector3(path[0].x, path[0].y, 0);
            transform.position = Vector2.MoveTowards(transform.position, pathPos, speed * Time.deltaTime);

            if (squad == null)
            {
                if (transform.position == pathPos)
                {
                    path.RemoveAt(0);
                }
            }
            else
            {
                float dist = Vector3.Distance(transform.position, pathPos);
                if (dist <= 1 + (squad.members.Count / 10))
                {
                    path.RemoveAt(0);
                }
            }
            float diff = pathPos.y - transform.position.y;
            anim.SetInteger("Direction", Mathf.RoundToInt(diff));


            // Damage building in path
            //RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(xDiff, yDiff), .2f, buildingMask);

            //if (hit.collider != null && !(target is Follower) && target.gameObject != hit.collider.gameObject)
            //{
            //    target = hit.collider.GetComponent<Interaction>();
            //}
        }
    }

    protected void Swarm()
    {
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
        health -= damage;

        if (!(target is Follower) && attacker != null)
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
            if (squad != null)
            {
                squad.SetTarget(attacker);
            }
            Pathfinding.FindPath(ref path, transform.position, target.transform.position);
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
            Pathfinding.FindPath(ref path, transform.position, target.transform.position);
        }
    }

    IEnumerator PathUpdate()
    {
        if (target != null && !target.staticObject)
        {
            // Update path less often when further away from the target (and only update path if target moves)
            yield return new WaitForSeconds(Vector3.Distance(transform.position, target.transform.position) / 100);
            if (target != null)
            {
                Pathfinding.FindPath(ref path, transform.position, target.transform.position);
            }
        }
        else
        {
            yield return null;
        }
        StartCoroutine(PathUpdate());
    }

    IEnumerator HitRoutine()
    {
        rend.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.color = Color.white;
        if (health <= 0)
        {
            Enemies.Remove(this);
            if (squad != null)
            {
                squad.RemoveMember(this);
            }
            Destroy(gameObject);
        }
    }
}
