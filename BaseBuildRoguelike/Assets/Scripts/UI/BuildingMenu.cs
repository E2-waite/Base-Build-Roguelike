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
        Spawner spawner = Spawner.Instance;
        buttons = new GameObject[spawner.buildings.Count];
        Vector2 pos = new Vector2(Screen.width - 25, 25);
        for (int i = 0; i < spawner.buildings.Count; i++)
        {
            buttons[i] = Instantiate(button, pos, Quaternion.identity);
            buttons[i].transform.SetParent(transform);
            buttons[i].GetComponent<Image>().sprite = spawner.buildings[i].sprite;
            pos.x -= 25;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (eventData.pointerCurrentRaycast.gameObject == buttons[i])
            {
                Spawner.Instance.selectedTemplate = i;
                GameController.Instance.gameState = GameController.GameState.build;
                return;
            }
        }
    }
}
