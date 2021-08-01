using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class GuardInspector : InspectorDetails, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    public GameObject button;
    public Image icon;
    GuardTower guardTower;

    public override int Reload(Interaction selected)
    {
        guardTower = selected as GuardTower;

        if (guardTower.archer == null)
        {
            icon.gameObject.SetActive(false);
        }
        else
        {
            icon.gameObject.SetActive(true);
            icon.sprite = Icons.Follower(Follower.Type.archer);
        }
        return 30;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerCurrentRaycast.gameObject == button)
        {
            if (guardTower.archer == null && Followers.selected != null && Followers.selected is Archer)
            {
                guardTower.AddArcher(Followers.selected as Archer);
                icon.sprite = Icons.Follower(Follower.Type.archer);
            }
            else if (guardTower.archer != null)
            {
                guardTower.RemoveArcher();
                icon.gameObject.SetActive(false);
            }
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        if (pointerEventData.pointerCurrentRaycast.gameObject == button && guardTower.archer == null && Followers.selected != null && Followers.selected is Archer)
        {
            icon.gameObject.SetActive(true);
            icon.sprite = Icons.Modify(0);
        }
        else if (pointerEventData.pointerCurrentRaycast.gameObject == button && guardTower.archer != null)
        {
            icon.sprite = Icons.Modify(1);
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        if (guardTower.archer == null)
        {
            icon.gameObject.SetActive(false);
        }
        else
        {
            icon.sprite = Icons.Follower(Follower.Type.archer);
        }
    }
}
