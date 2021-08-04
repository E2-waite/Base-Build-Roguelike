using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InventoryInspector : InspectorDetails
{
    public Text[] resourceVal = new Text[Resources.NUM];

    public void Reload(Inventory inventory)
    {
        for (int i = 0; i < Resources.NUM; i++)
        {
            resourceVal[i].text = inventory.resources[i].ToString();
        }
    }

}
