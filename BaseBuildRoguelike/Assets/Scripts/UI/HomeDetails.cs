using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class HomeDetails : MonoBehaviour, IPointerClickHandler
{
    HomeBase home;
    public GameObject spawnButton;
    public Text foodVal;
    public void Reload(Building building)
    {
        if (home == null)
        {
            home = (HomeBase)building;
        }

        foodVal.text = GameController.Instance.resources[(int)Resource.Type.food].ToString() + "/" + FollowerController.Instance.followerCost.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == spawnButton && home.SpawnFollower())
        {
            Debug.Log("Spawned");
        }
    }
}
