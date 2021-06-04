using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeBase : Building
{
    public int followerCost;
    public bool SpawnFollower()
    {
        if (GameController.Instance.food >= followerCost && !FollowerController.Instance.IsMaxFollowers() && BuildingController.Instance.UseResource(Resource.Type.food, 10))
        {
            FollowerController.Instance.SpawnFollower(new Vector3(transform.position.x, transform.position.y - 0.5f, 0));
            ReloadInspector();
            return true;
        }
        return false;
    }
}
