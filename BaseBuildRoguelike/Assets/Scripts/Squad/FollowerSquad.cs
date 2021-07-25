using UnityEngine;
public class FollowerSquad : Squad
{
    void Start()
    {
        Followers.squads.Add(this);
    }

    public override void Setup(Interaction member1, Interaction member2)
    {
        Follower follower1 = member1 as Follower, follower2 = member2 as Follower;
        members.Add(follower1);
        members.Add(follower2);
        follower1.squad = this;
        follower2.squad = this;
        Followers.selectedSquad = this;
        Inspector.Enable(follower1);
        Select();
    }

    public override void UpdateMembers()
    {
        for (int i = 0; i < members.Count; i++)
        {
            (members[i] as Follower).squad = this;
        }
    }


    public override void AddMember(Interaction newMember)
    {
        members.Add(newMember);
        (newMember as Follower).squad = this;
        Select();
    }

    public override void Select()
    {
        foreach (Follower follower in members)
        {
            if (follower != null)
            {
                follower.Select();
            }
        }
        Followers.selectedSquad = this;
        Inspector.Enable(members[0]);
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

    public void Direct(Vector2 pos, Interaction obj)
    {
        if (obj == null)
        {
            Move(pos);
        }
        else
        {
            if (obj is Enemy)
            {
                TargetEnemy(obj as Enemy);
            }
            else if (obj is Follower)
            {
                Merge(obj as Follower);
            }
        }
    }

    void Move(Vector2 pos)
    {
        for (int i = 0; i < members.Count; i++)
        {
            Follower follower = members[i] as Follower;
            follower.MoveTo(pos);
        }
    }

    void TargetEnemy(Enemy enemy)
    {
        for (int i = 0; i < members.Count; i++)
        {
            Follower follower = members[i] as Follower;
            follower.TargetEnemy(enemy);
        }
    }

    void Merge(Follower targetFollower)
    {
        for (int i = 0; i < members.Count; i++)
        {
            Follower follower = members[i] as Follower;
            follower.JoinSquad(targetFollower);
        }
    }

    public override void Disband()
    {
        Followers.squads.Remove(this);
        if (selected)
        {
            Followers.selected = members[0] as Follower;
            Followers.selected.squad = null;
            if (selected)
            {
                Inspector.Enable(Followers.selected);
            }
        }
        Destroy(gameObject);
    }
}
