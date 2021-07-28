using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
public abstract class Follower : Interaction
{
    public class Action
    {
        public Target target;
        public int state;
        public Action (Target _target, int _state)
        {
            target = _target;
            state = _state;
        }
    }

    public enum Type
    {
        worker = 0,
        soldier = 1,
        archer = 2,
        priest = 3
    }
    enum DefaultState
    {
        idle = 0,
        move = 1,
        attack = 2
    }

    [Header("Follower Settings")]
    public Type type;
    //public int state = 0;
    //public Target target = new Target();
    public Action currentAction = new Action(new Target(), 0);
    public List<Action> actions = new List<Action>();
    public FollowerSquad squad;
    public int maxHealth = 10, health, hitDamage = 1;
    public float targetDist = 0.25f, speed = 5f, targetRange = 15, chaseDist = 0.5f;
    public GameObject highlight, marker, squadPrefab, corpsePrefab, bloodEffect = null;
    public List<Vector2Int> path = new List<Vector2Int>();
    public Vector2Int currentPos;
    public Light2D torchLight;
    public bool selectable = true;
    protected Animator anim;
    protected SpriteRenderer rend;
    protected Coroutine interactRoutine = null;

    private void Start()
    {
        actions.Add(currentAction);
        currentPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        health = maxHealth;
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        torchLight = GetComponent<Light2D>();
        StartCoroutine(PathUpdate());
        Setup();
    }

    public virtual void Setup()
    {

    }

    public void Select()
    {
        highlight.SetActive(true);
    }

    public void Deselect()
    {
        highlight.SetActive(false);
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

    public IEnumerator LightFade(bool on)
    {
        Debug.Log("STARTING FADE");
        while ((on) ? torchLight.intensity < 0.75f : torchLight.intensity > 0)
        {
            torchLight.intensity = (on) ? torchLight.intensity + (0.5f * Time.deltaTime) : torchLight.intensity - (0.5f * Time.deltaTime);
            yield return null;
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
            squad = newSquad.GetComponent<FollowerSquad>();
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
        if (interactRoutine != null)
        {
            StopCoroutine(interactRoutine);
            interactRoutine = null;
        }

        // Clear the action list, before assigning a new action 
        actions = new List<Action>();
        actions.Add(new Action(new Target(), (int)DefaultState.idle));
        int state = 0;
        Target target = new Target();

        marker.transform.position = pos;
        if (Pathfinding.FindPath(ref path, currentPos, new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y))))
        {
            Debug.Log("Path Found");
        }
        else
        {
            Debug.Log("No Path Found");
        }

        if (obj != null)
        {
            target = new Target(obj);

            marker.transform.position = obj.transform.position;


            if (target.interact is Enemy)
            {
                state = (int)DefaultState.attack;
            }
            else if (target.interact is Follower)
            {
                Follower follower = target.interact as Follower;
                if (follower is Soldier || follower is Archer || follower is Priest)
                {
                    JoinSquad(follower);
                }
            }
        }
        else
        {
            target = new Target();
            state = (int)DefaultState.move; 
        }

        // Adds the action to the list of actions and sets as current target
        actions.Add(new Action(target, state));
        currentAction = actions[actions.Count - 1];
    }

    public virtual void BuildingDirect(Building building)
    {

    }

    public virtual IEnumerator PathUpdate()
    {
        if (currentAction.target.interact != null && currentAction.target.UpdatePath())
        {
            // Update path less often when further away from the target (and only update path if target moves)
            //yield return new WaitForSeconds(Vector3.Distance(transform.position, target.transform.position) / 100);
            Pathfinding.FindPath(ref path, currentPos, currentAction.target.Position2D());
        }
        else
        {
            yield return null;
        }
        StartCoroutine(PathUpdate());
    }

    public void MoveTo(Vector2 pos)
    {
        actions = new List<Action>();
        actions.Add(new Action(new Target(), (int)DefaultState.move));
        currentAction = actions[actions.Count - 1];
        marker.transform.position = pos;
        Pathfinding.FindPath(ref path, currentPos, new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)));
    }

    protected void Idle()
    {
        actions = new List<Action>();
        currentAction = new Action(new Target(), 0);
    }

    public void TargetEnemy(Enemy enemy)
    {
        if (enemy != null)
        {
            // Direct follower to target the input enemy
            if (interactRoutine != null)
            {
                StopCoroutine(interactRoutine);
                interactRoutine = null;
            }


            if (enemy.squad == null)
            {
                actions.Add(new Action(new Target(enemy), (int)DefaultState.attack));
            }
            else
            {
                // If target enemy is in a squad, instead target closest member of the squad
                actions.Add(new Action(new Target(enemy.squad.ClosestMember(transform.position)), (int)DefaultState.attack));
            }

            currentAction = actions[actions.Count - 1];
            marker.transform.position = currentAction.target.Position();
            Pathfinding.FindPath(ref path, currentPos, currentAction.target.Position2D());
        }
    }

    public bool Hit(int damage, Enemy attacker)
    {
        // Take damage
        health -= damage;
        StartCoroutine(HitRoutine());
        Bleed(attacker.transform.position);

        if (attacker != null && (currentAction.state == (int)DefaultState.idle || currentAction.state == (int)DefaultState.move) && (this is Soldier || this is Archer))
        {
            UpdateTarget(attacker);
        }

        if (health <= 0)
        {
            return true;
        }
        return false;
    }

    public void UpdateTarget(Interaction interaction, bool fromSquad = false)
    {
        if (currentAction.target.interact != null)
        {
            // Continue attacking current target
            return;
        }

        if (!fromSquad && squad != null)
        {
            // If called from a squad, do not attempt to update the target of squad members
            for (int i = 0; i < squad.members.Count; i++)
            {
                if (squad.members[i] != this)
                {
                    (squad.members[i] as Enemy).UpdateTarget(interaction, true);
                }
            }
        }

        // Target desired interaction
        actions.Add(new Action(new Target(), (int)DefaultState.attack));
        currentAction = actions[actions.Count - 1];

    }


    void Bleed(Vector3 hitPos)
    {
        var lookPos = transform.position - hitPos;
        Instantiate(bloodEffect, transform.position, Quaternion.LookRotation(lookPos));
    }

    IEnumerator HitRoutine()
    {
        // React to hit after delay
        rend.color = Color.red;

        if (Followers.selected == this || (squad != null && Followers.selectedSquad == squad))
        {
            Inspector.Enable(this);
        }

        yield return new WaitForSeconds(0.1f);
        rend.color = Color.white;
        if (health <= 0)
        {
            Instantiate(corpsePrefab, transform.position, Quaternion.identity);
            Followers.Remove(this);
            Followers.UpdateHUD();
            if (squad != null)
            {
                squad.RemoveMember(this);
            }

            if (Followers.selected == this && squad == null)
            {
                Inspector.Disable();
            }

            Destroy(gameObject);
        }
    }

    public bool Heal(int val)
    {
        Debug.Log("heal");
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

        if (Followers.selected == this || (squad != null && Followers.selectedSquad == squad))
        {
            Inspector.Enable(this);
        }

        return true;
    }
}
