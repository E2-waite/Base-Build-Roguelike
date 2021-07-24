using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Interaction
{
    [Header("Enemy Settings")]
    public int type;
    public int maxHealth = 3, health, hitDamage = 1;
    public float speed = 2, targetDist = 1f, targetRange = 15;
    public LayerMask buildingMask;
    public Squad squad;
    public Target target = new Target();
    public List<Target> targets = new List<Target>();
    public List<Vector2Int> path = new List<Vector2Int>();
    public Vector2Int currentPos;
    protected SpriteRenderer rend;
    protected Animator anim;
    public GameObject squadPrefab;
    private void Start()
    {
        currentPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        targets.Add(new Target(Buildings.homeBase));
        Target newTarget = new Target();
        if (Targetting.FindTarget(ref newTarget, squad, transform.position, Followers.followers))
        {
            targets.Add(newTarget);
        }
        target = targets[targets.Count - 1];

        FindPath();

        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        health = maxHealth;
        
        StartCoroutine(PathUpdate());
    }

    void FindPath()
    {
        // Find path and add any buildings blocking the way to the targets list.
        List<Target> newTargets = new List<Target>();
        Pathfinding.FindPath(ref path, ref newTargets, currentPos, target.Position2D(), 1);
        if (newTargets.Count > 0)
        {
            targets.AddRange(newTargets);
            target = targets[targets.Count - 1];
        }
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
                    currentPos = path[0];
                    path.RemoveAt(0);
                }
            }
            else
            {
                float dist = Vector3.Distance(transform.position, pathPos);
                if (dist <= 1 + (squad.members.Count / 10))
                {
                    currentPos = path[0];
                    path.RemoveAt(0);
                }
            }
            float diff = pathPos.y - transform.position.y;
            anim.SetInteger("Direction", Mathf.RoundToInt(diff));
        }
    }

    protected void UpdateTarget()
    {
        Target newTarget = new Target();
        if (Targetting.FindTarget(ref newTarget, squad, transform.position, Followers.followers))
        {
            targets.Add(newTarget);
        }
        else
        {
            for (int i = targets.Count - 1; i >= 0; i--)
            {
                if (targets[i].interact == null)
                {
                    targets.RemoveAt(i);
                }
                else
                {
                    newTarget = targets[i];
                    break;
                }
            }
        }
        target = newTarget;
        FindPath();
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
    public bool Hit(int damage, Interaction attacker)
    {
        health -= damage;

        if (!(target.interact is Follower) && attacker != null)
        {
            // Update target if current target is not an enemy (stops switching target while in combat)
            target = new Target(attacker);
            targets.Add(target);
            if (squad != null)
            {
                squad.SetTarget(attacker);
            }
            FindPath();
            HitReaction(attacker.transform.position);
        }

        StartCoroutine(HitRoutine());

        if (health <= 0)
        {
            return true;
        }
        return false;
    }

    protected virtual void HitReaction(Vector3 hitPos)
    {

    }

    public void UpdateTarget(Interaction newTarget)
    {
        if (!(target.interact is Follower))
        {
            target = new Target(newTarget);
            FindPath();
        }
    }

    IEnumerator PathUpdate()
    {
        if (target != null && target.interact != null && target.UpdatePath())
        {
            FindPath();
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
