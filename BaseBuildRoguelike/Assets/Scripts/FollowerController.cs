using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowerController : MonoSingleton<FollowerController>
{
    public GameObject followerPrefab;
    List<Follower> followers = new List<Follower>();
    public Follower selected;

    public void SpawnFollower(Vector2 pos)
    {
        GameObject follower = Instantiate(followerPrefab, pos, Quaternion.identity);
        followers.Add(follower.GetComponent<Follower>());
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
            selected.Direct(pos, obj);
        }
    }


}
