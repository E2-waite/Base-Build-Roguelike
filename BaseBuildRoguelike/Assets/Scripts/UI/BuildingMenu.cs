using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingMenu : MonoBehaviour, IPointerClickHandler
{
    public List<GameObject> buildings = new List<GameObject>();

    public void OnPointerClick(PointerEventData eventData)
    {
        for (int i = 0; i < buildings.Count; i++)
        {
            if (eventData.pointerCurrentRaycast.gameObject == buildings[i])
            {
                BuildingController.Instance.selectedTemplate = BuildingController.Instance.buildingTypes[i];
                GameController.Instance.mode = GameController.Mode.build;
                return;
            }
        }
        
    }
}
