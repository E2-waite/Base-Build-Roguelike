using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InspectorContruction : InspectorDetails
{
    public Text woodText, stoneText;
    public override void Reload(Interaction selected)
    {
        Construct construct = (selected as Building).construct;
        woodText.text = (construct.woodCost - construct.woodRemaining).ToString() + "/" + construct.woodCost.ToString();
        stoneText.text = (construct.stoneCost - construct.stoneRemaining).ToString() + "/" + construct.stoneCost.ToString();
    }
}
