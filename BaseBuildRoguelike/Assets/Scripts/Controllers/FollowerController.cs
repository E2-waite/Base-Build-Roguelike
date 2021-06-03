using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerController : MonoSingleton<FollowerController>
{
    public GameObject followerPrefab;
    List<Follower> followers = new List<Follower>();
    public Follower selected;
    public int maxFollowers = 1;

    public void SpawnFollower(Vector2 pos)
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

    public void SelectFollower(Collider2D follower)
    {
        if (selected != null)
        {
            selected.Deselect();
        }
        selected = follower.GetComponent<Follower>();
        selected.Select();
    }

    public void DeselectFollower()
    {
        if (selected != null)
        {
            selected.Deselect();
            selected = null;
        }
    }

    public void DirectFollower(Vector2 pos, GameObject obj)
    {
        if (selected != null)
        {
            if (selected.type == Follower.Type.worker)
            {
                Worker worker = (Worker)selected;
                worker.Direct(pos, obj);
            }
            else if (selected.type == Follower.Type.soldier)
            {
                Soldier soldier = (Soldier)selected;
                soldier.Direct(pos, obj);
            }
        }
    }


}
