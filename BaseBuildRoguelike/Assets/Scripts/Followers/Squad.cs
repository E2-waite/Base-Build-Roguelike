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
        defend
    }
    public State state = State.idle;
    public GameObject marker;
    public int maxFollowers = 5;
    public List<Follower> followers = new List<Follower>();
    public Interaction target;
    public void Setup(Follower follower1, Follower follower2)
    {
        followers.Add(follower1);
        followers.Add(follower2);
        follower1.squad = this;
        follower2.squad = this;
    }

    public bool AddFollower(Follower follower)
    {
        if (followers.Count < maxFollowers)
        {
            follower.squad = this;
            followers.Add(follower);
            return true;
        }
        return false;
    }

    public void Direct(Vector2 pos, GameObject obj)
    {
        marker.transform.position = pos;

        if (obj != null)
        {
            marker.transform.position = obj.transform.position;
            target = obj.GetComponent<Interaction>();
        }
        else
        {
            marker.transform.position = pos;
            target = null;
        }

        foreach (Follower follower in followers)
        {
            if (follower != null)
            {
                if (follower.type == Follower.Type.soldier)
                {
                    Soldier soldier = (Soldier)follower;
                    soldier.Direct(pos, target);
                }
                else if (follower.type == Follower.Type.archer)
                {
                    Archer archer = (Archer)follower;
                    archer.Direct(pos, target);
                }
            }
        }

    }
}
