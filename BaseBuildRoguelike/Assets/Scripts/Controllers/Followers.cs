using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  static class Followers
{
    public static List<Follower> followers = new List<Follower>();
    public static List<FollowerSquad> squads = new List<FollowerSquad>();
    public static Follower selected;
    public static FollowerSquad selectedSquad = null;
    public static int maxFollowers = 1;

    public static void Reset()
    {
        followers = new List<Follower>();
        squads = new List<FollowerSquad>();
        selected = null;
        selectedSquad = null;
        maxFollowers = 1;
    }

    public static void Add(Follower follower)
    {
        followers.Add(follower);
    }
    public static void Remove(Follower follower)
    {
        followers.Remove(follower);
        if (followers.Count == 0 && Resources.resources[(int)Resource.Type.food] >= 10)
        {
            GameController.Instance.GameOver();
        }
    }
    public static int Count()
    {
        return followers.Count;
    }

    public static int Max()
    {
        return maxFollowers;
    }

    public static bool AtCapacity()
    {
        if (followers.Count >= maxFollowers)
        {
            return true;
        }
        return false;
    }

    public static void AdjustMaxFollowers(int val)
    {
        maxFollowers += val;
        HUD.Instance.UpdateFollowers(followers.Count, maxFollowers);
    }

    public static void UpdateHUD()
    {
        HUD.Instance.UpdateFollowers(followers.Count, maxFollowers);
    }

    public static void Direct(Vector2 pos, GameObject obj)
    {
        Interaction objInteraction = null;
        if (obj != null)
        {
            objInteraction = obj.GetComponent<Interaction>();
        }

        if (selectedSquad != null)
        {
            selectedSquad.Direct(pos, objInteraction);
        }
        else if (selected != null)
        {
            selected.Direct(pos, objInteraction);
        }
    }

    public static void Select(Follower follower)
    {
        if (selected != null)
        {
            selected.Deselect();
        }
        if (selectedSquad != null)
        {
            selectedSquad.Deselect();
            selectedSquad = null;
        }

        if (follower.selectable)
        {
            selected = follower;
            if (selected.squad != null)
            {
                selectedSquad = selected.squad;
                selectedSquad.Select();
            }
            else
            {
                selected.Select();
            }
        }
    }

    public static void Deselect()
    {
        if (selectedSquad != null)
        {
            selectedSquad.Deselect();
            selected = null;
            selectedSquad = null;
        }
        else if (selected != null)
        {
            selected.Deselect();
            selected = null;
            selectedSquad = null;
        }
    }

    public static Interaction Closest(Vector3 pos)
    {
        Interaction closest = null;
        float closestDist = 9999;
        for (int i = 0; i < followers.Count; i++)
        {
            if (followers[i] != null)
            {
                float dist = Vector3.Distance(pos, followers[i].transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = followers[i];
                }
            }
        }
        return closest;
    }
}