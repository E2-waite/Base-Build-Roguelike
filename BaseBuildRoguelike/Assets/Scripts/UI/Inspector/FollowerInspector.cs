using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FollowerInspector : InspectorDetails
{
    public Image icon, healthBar;
    public override int Reload(Interaction selected)
    {
        Follower follower = selected as Follower;
        icon.sprite = Icons.Follower(follower.type);
        healthBar.fillAmount = (float)follower.health / (float)follower.maxHealth;
        return 0;
    }
}
