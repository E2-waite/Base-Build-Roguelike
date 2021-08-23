using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeBase : Building
{
    public int followerCost;
    public bool SpawnFollower()
    {
        if (Resources.Value(Resource.Type.food) >= followerCost && !Followers.AtCapacity() && Buildings.UseResource(Resource.Type.food, 10))
        {
            Spawner.Instance.SpawnFollower(new Vector3(transform.position.x, transform.position.y - 0.5f, 0));
            ReloadInspector();
            return true;
        }
        return false;
    }

    public override void Destroy()
    {
        GameController.Instance.GameOver();
        base.Destroy();
    }
}
