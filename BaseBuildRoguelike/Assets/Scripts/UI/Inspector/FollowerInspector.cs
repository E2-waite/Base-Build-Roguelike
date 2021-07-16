using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FollowerInspector : InspectorDetails
{
    public Image icon;
    public override int Reload(Interaction selected)
    {
        Follower follower = selected as Follower;
        icon.sprite = Icons.Follower(follower.type);
        return 0;
    }
}
