using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Squad : MonoBehaviour
{
    public int maxFollowers = 5, targetRange = 15;
    public List<Interaction> members = new List<Interaction>();
    public bool selected = false;

    /// <summary>Set up a new squad from two members with no existing squad</summary>
    public virtual void Setup(Interaction member1, Interaction member2)
    {

    }

    /// <summary>Load squad from member indexes</summary>
    public void Setup(int[] memberIndexes)
    {
        for (int i = 0; i < memberIndexes.Length; i++)
        {
            members.Add(Grid.TargetFromIndex(memberIndexes[i]));
        }

        UpdateMembers();
    }

    /// <summary>Updates the squad reference for all members in the squad</summary>
    public virtual void UpdateMembers()
    {

    }

    /// <summary>Add new member to the squad</summary>
    public virtual void AddMember(Interaction newMember)
    {

    }

    public void Combine(Squad squad)
    {
        if (squad != this)
        {
            members.AddRange(squad.members);
            UpdateMembers();
            Select();
        }
    }

    public virtual void Select()
    {

    }

    public void RemoveMember (Interaction member)
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
            Disband();
        }
        if (selected)
        {
            Inspector.Enable(members[0]);
        }
    }

    public virtual void Disband()
    {

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
}
