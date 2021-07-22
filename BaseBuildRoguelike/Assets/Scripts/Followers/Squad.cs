using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squad : MonoBehaviour
{
    public enum State
    {
        idle,
        move,
        attack,
        defend,
        merge
    }

    public enum Type
    {
        friendly,
        hostile
    }

    public Type type = Type.friendly;
    public State state = State.idle;
    public GameObject marker;
    public int maxFollowers = 5, targetRange = 15;
    public List<Interaction> members = new List<Interaction>();
    public Target target = null;
    public bool selected = false;

    private void Start()
    {
        if (type == Type.friendly)
        {
            Followers.squads.Add(this);
        }
        else
        {
            Enemies.squads.Add(this);
        }
    }

    public void Setup(Follower follower1, Follower follower2)
    {
        members.Add(follower1);
        members.Add(follower2);
        follower1.squad = this;
        follower2.squad = this;
        Followers.selectedSquad = this;
        Inspector.Enable(follower1);
        Select();
    }

    public void Setup(Enemy enemy1, Enemy enemy2)
    {
        members.Add(enemy1);
        members.Add(enemy2);
        enemy1.squad = this;
        enemy2.squad = this;
        type = Type.hostile;
    }

    public void Setup(int[] memberIndexes)
    {
        for (int i = 0; i < memberIndexes.Length; i++)
        {
            members.Add(Grid.TargetFromIndex(memberIndexes[i]));
        }

        if (members[0] is Follower)
        {
            type = Type.friendly;
        }
        else
        {
            type = Type.hostile;
        }

        for (int i = 0; i < members.Count; i++)
        {
            if (type == Type.friendly)
            {
                (members[i] as Follower).squad = this;
            }
            else
            {
                (members[i] as Enemy).squad = this;
            }
        }
    }

    public void AddMember(Follower follower)
    {
        follower.squad = this;
        members.Add(follower);
        Followers.selectedSquad = this;
        Inspector.Enable(follower);
        Select();
    }

    public void AddMember(Enemy enemy)
    {
        enemy.squad = this;
        members.Add(enemy);
    }

    public void Combine(Squad squad)
    {
        if (squad != this)
        {
            foreach (Interaction member in squad.members)
            {
                if (member != null)
                {
                    if (member is Follower)
                    {
                        (member as Follower).squad = this;
                    }
                    else if (member is Enemy)
                    {
                        (member as Enemy).squad = this;
                    }
                }
            }
            members.AddRange(squad.members);
            Destroy(squad.gameObject);

            if (type == Type.friendly)
            {
                Followers.selectedSquad = this;
                Select();
                Inspector.Enable(members[0]);
            }
        }
    }

    public void Direct(Vector2 pos, Interaction obj)
    {
        marker.transform.position = pos;

        if (obj == null)
        {
            state = State.move;
            marker.transform.position = pos;
            target = null;
        }
        else
        {
            marker.transform.position = obj.transform.position;

            if (obj is Enemy)
            {
                state = State.attack;
                Enemy enemy = obj as Enemy;
                target = new Target(enemy);
            }
            else if (obj is Follower)
            {
                state = State.merge;
                target = new Target(obj);
            }
        }

        DirectSquad(pos);
    }

    void DirectSquad(Vector2 pos)
    {
        foreach (Follower follower in members)
        {
            if (follower != null)
            {
                if (state == State.move)
                {
                    follower.MoveTo(pos);
                }
                else if (state == State.attack)
                {
                    if (target.squad == null)
                    {
                        follower.TargetEnemy(target.interact as Enemy);
                    }
                    else
                    {
                        follower.TargetEnemy(target.squad.ClosestMember(pos) as Enemy);
                    }
                }
                else if (state == State.merge)
                {
                    follower.JoinSquad(target.interact as Follower);
                    return;
                }
            }
        }
    }

    public void Select()
    {
        foreach (Follower follower in members)
        {
            if (follower != null)
            {
                follower.Select();
            }
        }
        selected = true;
    }

    public void Deselect()
    {
        foreach (Follower follower in members)
        {
            if (follower != null)
            {
                follower.Deselect();
            }
        }
        selected = false;
    }

    public bool RemoveMember (Interaction member)
    { 
        members.Remove(member);
        if (member is Follower)
        {
            Follower follower = member as Follower;
            follower.Deselect();
            follower.squad = null;
        }

        if (members.Count == 1)
        {
            if (selected && type == Type.friendly)
            {
                Followers.selected = members[0] as Follower;
                Followers.selected.squad = null;
                if (selected)
                {
                    Inspector.Enable(Followers.selected);
                }
            }
            Destroy(gameObject);
            return true;
        }
        if (selected)
        {
            Inspector.Enable(members[0]);
        }
        return false;
    }



    public Interaction ClosestMember(Vector3 pos)
    {
        Interaction member = null;
        float closestDist = 9999;
        for (int i = 0; i < members.Count; i++)
        {
            if (members[i] == null)
            {
                members.RemoveAt(i);
            }
            else
            {
                float dist = Vector3.Distance(pos, members[i].transform.position);
                if (dist < closestDist)
                {
                    member = members[i];
                    closestDist = dist;
                }
            }
        }
        return member;
    }

    public void SetTarget (Interaction newTarget)
    {
        if (newTarget is Enemy)
        {
            target = new Target(newTarget);

            foreach (Interaction member in members)
            {
                Debug.Log("Setting Target");
                (member as Follower).TargetEnemy(target.interact as Enemy);
            }
        }
        else
        {
            target = new Target(newTarget);

            foreach (Interaction member in members)
            {
                if (target.squad == null)
                {
                    (member as Enemy).UpdateTarget(target.interact as Follower);
                }
                else
                {
                    (member as Enemy).UpdateTarget(target.squad.ClosestMember(member.transform.position));
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (type == Type.friendly)
        {
            Followers.squads.Remove(this);
        }
        else
        {
            Enemies.squads.Remove(this);
        }
    }
}
