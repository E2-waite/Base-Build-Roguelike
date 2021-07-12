using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Follower : Interaction
{
    public enum State
    {
        idle,
        move,
        chopWood,
        mineStone,
        store,
        build,
        hunt,
        attack,
        defend
    }
    [Header("Follower Settings")]
    public State state = State.idle;
    public Interaction target;
    public Squad squad, targetSquad;
    public int maxHealth = 10, health, hitDamage = 1;
    public float targetDist = 0.25f, speed = 5f, targetRange = 15, chaseDist = 0.5f;
    public bool canAttack = true, attacking = false;
    public GameObject highlight, marker, glow, squadPrefab, corpsePrefab;
    bool selected;
    public List<Vector2Int> path = new List<Vector2Int>();
    protected Animator anim;
    protected SpriteRenderer rend;

    private void Start()
    {
        health = maxHealth;
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        StartCoroutine(PathUpdate());
        Setup();
    }

    public virtual void Setup()
    {

    }

    public void Select()
    {
        highlight.SetActive(true);
        selected = true;
    }

    public void Deselect()
    {
        highlight.SetActive(false);
        selected = false;
    }

    public void Move()
    {
        // Move towards position and keep distance from other followers in squad
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
        }
    }



    public void Move(Vector3 position)
    {
        // Move towards position and keep distance from other followers in squad
        transform.position = Vector2.MoveTowards(transform.position, position, speed * Time.deltaTime);
        float diff = position.y - transform.position.y;
        anim.SetInteger("Direction", Mathf.RoundToInt(diff));
    }

    protected void Swarm()
    {
        if (squad != null)
        {
            foreach (Follower follower in squad.members)
            {
                if (follower != this && follower != null)
                {
                    float dist = Vector3.Distance(transform.position, follower.transform.position);
                    if (dist <= 0.5f)
                    {
                        Vector3 diffVec = transform.position - follower.transform.position;
                        transform.position = Vector2.MoveTowards(transform.position, transform.position + diffVec, speed * Time.deltaTime);
                    }
                }
            }
        }
    }

    public void JoinSquad(Follower follower)
    {
        // Join/Merge squads
        if (follower.squad == null && squad == null)
        {
            // No squad - create one
            GameObject newSquad = Instantiate(squadPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            squad = newSquad.GetComponent<Squad>();
            squad.Setup(this, follower);
            return;
        }
        else if (follower.squad != null && squad == null)
        {
            follower.squad.AddMember(this);
            return;
        }
        else if (follower.squad == null && squad != null)
        {
            squad.AddMember(follower);
            return;
        }
        else
        {
            squad.Combine(follower.squad);
        }
    }


    public virtual void Direct(Vector2 pos, Interaction obj)
    {
        // Standard combat based direct (overridden by followers with unique functionality i.e. priests and workers)
        canAttack = true;
        attacking = false;
        marker.transform.position = pos;
        if (Pathfinding.FindPath(ref path, transform.position, pos))
        {
            Debug.Log("Path Found");
        }
        else
        {
            Debug.Log("No Path Found");
        }

        if (obj != null)
        {
            target = obj;

            marker.transform.position = obj.transform.position;


            if (target is Enemy)
            {
                state = State.attack;
                Enemy enemy = target as Enemy;
                if (enemy.squad != null)
                {
                    targetSquad = enemy.squad;
                }
            }
            else if (target is Building)
            {
                if ((target as Building).isConstructed)
                {
                    state = State.defend;
                }
            }
            else if (target is Follower)
            {
                Follower follower = target as Follower;
                if (follower is Soldier || follower is Archer)
                {
                    JoinSquad(follower);
                }
            }
        }
        else
        {
            target = null;
            state = State.move;
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

    public void MoveTo(Vector2 pos)
    {
        target = null;
        marker.transform.position = pos;
        Pathfinding.FindPath(ref path, transform.position, pos);
        state = State.move;
    }

    public void TargetEnemy(Enemy enemy)
    {
        if (enemy != null)
        {
            // Direct follower to target the input enemy
            canAttack = true;
            attacking = false;
            if (enemy.squad == null)
            {
                target = enemy;
                targetSquad = null;
            }
            else
            {
                // If target enemy is in a squad, instead target closest member of the squad
                targetSquad = enemy.squad;
                target = targetSquad.ClosestMember(transform.position);
            }
            marker.transform.position = target.transform.position;
            Pathfinding.FindPath(ref path, transform.position, target.transform.position);
            state = State.attack;
        }
    }

    public bool Hit(int damage, Enemy attacker)
    {
        // Take damage
        health -= damage;
        StartCoroutine(HitRoutine());


        if ((state == State.idle || state == State.move) && (this is Soldier || this is Archer))
        {
            TargetEnemy(attacker);
            if (squad != null)
            {
                squad.SetTarget(attacker);
            }
        }

        if (health <= 0)
        {
            return true;
        }
        return false;
    }

    IEnumerator HitRoutine()
    {
        // React to hit after delay
        rend.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.color = Color.white;
        if (health <= 0)
        {
            Instantiate(corpsePrefab, transform.position, Quaternion.identity);
            FollowerController.Instance.Remove(this);
            if (squad != null)
            {
                squad.RemoveMember(this);
            }
            Destroy(gameObject);
        }
    }

    public bool Heal(int val)
    {
        Debug.Log("heal");
        StartCoroutine(GlowRoutine());
        if (health == maxHealth)
        {
            return false;
        }
        else if (health + val > maxHealth)
        {
            health = maxHealth;
        }
        else
        {
            health += val;
        }
        return true;
    }

    IEnumerator GlowRoutine()
    {
        glow.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        glow.SetActive(false);
    }
}
