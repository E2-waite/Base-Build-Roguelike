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
    public bool selected = false;
    public void Setup(Follower follower1, Follower follower2)
    {
        followers.Add(follower1);
        followers.Add(follower2);
        follower1.squad = this;
        follower2.squad = this;
        FollowerController.Instance.selectedSquad = this;
        Select();
    }

    public void AddFollower(Follower follower)
    {
        follower.squad = this;
        followers.Add(follower);
        FollowerController.Instance.selectedSquad = this;
        Select();
    }

    public void Combine(Squad squad)
    {
        if (squad != this)
        {
            foreach (Follower follower in squad.followers)
            {
                if (follower != null)
                {
                    follower.squad = this;
                }
            }
            followers.AddRange(squad.followers);
            Destroy(squad.gameObject);
            FollowerController.Instance.selectedSquad = this;
            Select();
        }
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
                follower.Direct(pos, target);

                // If target is a follower - only direct the first follower in the squad (prevents multiple instances of squad combination)
                if (target != null && target is Follower)
                {
                    return;
                }
            }
        }

    }

    public void Select()
    {
        foreach (Follower follower in followers)
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
        foreach (Follower follower in followers)
        {
            if (follower != null)
            {
                follower.Deselect();
            }
        }
        selected = false;
    }
}
