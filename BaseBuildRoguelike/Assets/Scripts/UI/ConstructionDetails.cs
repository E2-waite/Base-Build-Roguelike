using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ConstructionDetails : MonoBehaviour
{
    public Text woodText, stoneText;

    public void Reload(Building building)
    {
        Debug.Log(building.name);
        woodText.text = (building.construct.woodCost - building.construct.woodRemaining).ToString() + "/" + building.construct.woodCost.ToString();
        stoneText.text = (building.construct.stoneCost - building.construct.stoneRemaining).ToString() + "/" + building.construct.stoneCost.ToString();
    }
}
