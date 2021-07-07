using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingMenu : MonoBehaviour, IPointerClickHandler
{
    GameObject[] buttons;
    public GameObject button;

    private void Start()
    {
        BuildingController controller = BuildingController.Instance;
        buttons = new GameObject[controller.buildingTypes.Count];
        Vector2 pos = new Vector2(Screen.width - 25, 25);
        for (int i = 0; i < controller.buildingTypes.Count; i++)
        {
            buttons[i] = Instantiate(button, pos, Quaternion.identity);
            buttons[i].transform.parent = transform;
            buttons[i].GetComponent<Image>().sprite = controller.buildingTypes[i].sprite;
            pos.x -= 25;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (eventData.pointerCurrentRaycast.gameObject == buttons[i])
            {
                BuildingController.Instance.selectedTemplate = BuildingController.Instance.buildingTypes[i];
                GameController.Instance.mode = GameController.Mode.build;
                return;
            }
        }
        
    }
}
