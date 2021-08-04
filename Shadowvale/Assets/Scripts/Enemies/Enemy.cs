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
    public Action currentAction = new Action();
    public List<Action> actions = new List<Action>();
    public List<Vector2Int> path = new List<Vector2Int>();
    public Vector2Int currentPos;
    protected SpriteRenderer rend;
    protected Animator anim;
    public GameObject squadPrefab;
    protected AIData aiData;
    private void Start()
    {
        currentPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        actions.Add(new Action(new Target(Buildings.homeBase)));
        currentAction = actions[actions.Count - 1];

        FindPath();

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

        if (attacker != null)
        {
            UpdateTarget(attacker);
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

    protected void UpdateTarget()
    {
        if (squad != null)
        {

        }
    }

    protected void PreviousTarget()
    {
        // Loop through previous targets, selecting the first suitable target
        Action newAction = new Action();
        for (int i = actions.Count - 1; i >= 0; i--)
        {
            if (actions[i].target.interact == null)
            {
                actions.RemoveAt(i);
            }
            else
            {
                newAction = actions[i];
                break;
            }
        }

        currentAction = newAction;
        FindPath();
    }

    public void UpdateTarget(Interaction interaction, bool fromSquad = false)
    {
        if (currentAction.target.interact != null && !(currentAction.target.interact is Building))
        {
            // Continue attacking current target
            return;
        }

        if  (!fromSquad && squad != null)
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
        currentAction = new Action(new Target(interaction));
        actions.Add(currentAction);
        FindPath();
    }

    public void FindPath()
    {
        // Find path and add any buildings blocking the way to the targets list.
        if (currentAction.target.interact != null)
        {
            List<Action> newTargets = new List<Action>();
            Pathfinding.FindPath(ref path, ref newTargets, currentPos, currentAction.target.Position2D(), 1);
            if (newTargets.Count > 0)
            {
                actions.AddRange(newTargets);
                currentAction = actions[actions.Count - 1];
            }
        }
    }



    IEnumerator PathUpdate()
    {
        if (currentAction != null && currentAction.target.interact != null && currentAction.target.UpdatePath())
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

    public virtual void Save(AIData data)
    {
        data.type = type;
        data.health = health;
        data.pos = transform.position;
        data.gridPos = currentPos;
        data.statusEffects = new StatusEffectData(statusEffects);

        data.numActions = actions.Count;
        data.targets = new int[data.numActions];
        data.states = new int[data.numActions];

        for (int i = 0; i < data.numActions; i++)
        {
            actions[i].Save();
            data.targets[i] = actions[i].targetInd;
            data.states[i] = actions[i].state;
        }
    }

    public virtual void Load(AIData data)
    {
        type = data.type;
        health = data.health;
        transform.position = data.pos;
        currentPos = data.gridPos;
        if (data.statusEffects != null)
        {
            statusEffects = data.statusEffects.Read(this);
            glow.SetupGlow(statusEffects);
        }
        Enemies.Add(this);
        aiData = data;
    }

    public void SetTargets()
    {
        if (aiData.numActions > 0)
        {
            for (int j = 0; j < aiData.numActions; j++)
            {
                actions.Add(new Action(aiData.targets[j], aiData.states[j]));
            }
            currentAction = actions[actions.Count - 1];
        }
        else
        {
            currentAction = new Action();
        }
        aiData = null;
    }
}
