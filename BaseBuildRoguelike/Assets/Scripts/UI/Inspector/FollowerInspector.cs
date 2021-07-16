using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FollowerInspector : InspectorDetails
{
    public Image icon;
    public override void Reload(Interaction selected)
    {
        Follower follower = selected as Follower;
        icon.sprite = Icons.Follower(follower.type);
    }
}
