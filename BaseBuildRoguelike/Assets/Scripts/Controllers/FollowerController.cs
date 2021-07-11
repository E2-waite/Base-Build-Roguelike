using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerController : MonoSingleton<FollowerController>
{
    public int followerCost = 10;
    public GameObject followerPrefab;
    public List<Interaction> followers = new List<Interaction>();
    public Follower selected;
    public Squad selectedSquad = null;
    public int maxFollowers = 1;

    private void Start()
    {
        GameObject[] followerObjs = GameObject.FindGameObjectsWithTag("Follower");
        foreach(GameObject follower in followerObjs)
        {
            followers.Add(follower.GetComponent<Interaction>());
        }
    }

    public void SpawnFollower(Vector3 pos)
    {
        GameObject follower = Instantiate(followerPrefab, pos, Quaternion.identity);
        followers.Add(follower.GetComponent<Worker>());
        HUD.Instance.UpdateFollowers(followers.Count, maxFollowers);
    }

    public void AdjustMaxFollowers(int val)
    {
        maxFollowers += val;
        HUD.Instance.UpdateFollowers(followers.Count, maxFollowers);
    }

    public bool IsMaxFollowers()
    {
        if (followers.Count >= maxFollowers)
        {
            return true;
        }
        return false;
    }

    public void SelectFollower(Collider2D follower)
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

        selected = follower.GetComponent<Follower>();
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

    public void DeselectFollower()
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

    public void DirectFollower(Vector2 pos, GameObject obj)
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
    public void Remove(Follower follower)
    {
        followers.Remove(follower);
    }
}
