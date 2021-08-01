using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class HomeInspector : InspectorDetails, IPointerClickHandler
{
    HomeBase home;
    public GameObject spawnButton;
    public Text foodVal;

    public override int Reload(Interaction selected)
    {
        if (home == null)
        {
            home = (HomeBase)selected;
        }

        foodVal.text = Resources.Value(Resource.Type.food).ToString() + "/" + Spawner.Instance.followerCost.ToString();

        return 70;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == spawnButton && home.SpawnFollower())
        {
            Debug.Log("Spawned");
        }
    }
}
