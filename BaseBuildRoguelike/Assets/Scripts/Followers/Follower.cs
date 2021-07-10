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
    public float targetDist = 0.25f, speed = 5f, targetRange = 15;
    public bool canAttack = true;
    public GameObject highlight, marker, squadPrefab, corpsePrefab;
    bool selected;

    protected Animator anim;
    protected SpriteRenderer rend;

    private void Start()
    {
        health = maxHealth;
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
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

    public void Move(Vector3 position)
    {
        // Move towards position and keep distance from other followers in squad
        transform.position = Vector2.MoveTowards(transform.position, position, speed * Time.deltaTime);
        float diff = position.y - transform.position.y;
        anim.SetInteger("Direction", Mathf.RoundToInt(diff));
        if (squad != null)
        {
            foreach (Follower follower in squad.members)
            {
                if (follower != this && follower != null)
                {
                    float dist = Vector3.Distance(transform.position, follower.transform.position);
                    if (dist <= 0.5)
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
            follower.squad.AddFollower(this);
            return;
        }
        else if (follower.squad == null && squad != null)
        {
            squad.AddFollower(follower);
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
        marker.transform.position = pos;
        if (obj != null)
        {
            Debug.Log("Target: " + obj.name);
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

    public void MoveTo(Vector2 pos)
    {
        target = null;
        marker.transform.position = pos;
        state = State.move;
    }

    public void TargetEnemy(Enemy enemy)
    {
        // Direct follower to target the input enemy
        canAttack = true;
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
        state = State.attack;
    }

    public bool Hit(int damage, Enemy attacker)
    {
        // Take damage
        health -= damage;
        StartCoroutine(HitRoutine());

        if (this is Soldier)
        {
            ((Soldier)this).TargetEnemy(attacker);
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
            Destroy(gameObject);
        }
    }
}
