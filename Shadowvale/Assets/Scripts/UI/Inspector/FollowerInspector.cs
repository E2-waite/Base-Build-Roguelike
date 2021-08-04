using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FollowerInspector : InspectorDetails
{
    public Image icon, healthBar;
    public InventoryInspector InventoryInspector;
    public override int Reload(Interaction selected)
    {
        Follower follower = selected as Follower;
        icon.sprite = Icons.Follower(follower.type);
        healthBar.fillAmount = (float)follower.health / (float)follower.maxHealth;
        if (follower is Worker)
        {
            InventoryInspector.gameObject.SetActive(true);
            InventoryInspector.Reload((follower as Worker).inventory);
            return 100;
        }    
        else
        {
            InventoryInspector.gameObject.SetActive(false);
        }
        return 0;
    }
}
