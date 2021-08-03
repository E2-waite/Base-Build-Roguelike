using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingMenu : MonoBehaviour, IPointerClickHandler
{
    GameObject[] buttons;
    public GameObject button;
    public Sprite cancelIcon;
    public RectTransform mainBackground, endBackground, mainTransform;
    public GameObject toggleButton;
    public int totalPos = 0;
    public float toggleSpeed = 1000;
    public bool toggling = false, open = true;
    private void Start()
    {
        Spawner spawner = Spawner.Instance;
        buttons = new GameObject[spawner.buildings.Count + 1];
        Vector2 pos = new Vector2(-5, 25);
        int byPos = 30;
        for (int i = 0; i < spawner.buildings.Count; i++)
        {
            buttons[i] = Instantiate(button, Vector3.zero, Quaternion.identity);
            buttons[i].transform.SetParent(transform.GetChild(0));
            buttons[i].GetComponent<Image>().sprite = spawner.buildings[i].sprite;
            buttons[i].GetComponent<RectTransform>().anchoredPosition = pos;
            totalPos += byPos;
            pos.x -= byPos;
        }
        buttons[spawner.buildings.Count] = Instantiate(button, Vector3.zero, Quaternion.identity);
        buttons[spawner.buildings.Count].transform.SetParent(transform.GetChild(0));
        buttons[spawner.buildings.Count].GetComponent<RectTransform>().sizeDelta = new Vector3(25, 25, 0);
        buttons[spawner.buildings.Count].GetComponent<Image>().sprite = cancelIcon;
        buttons[spawner.buildings.Count].GetComponent<RectTransform>().anchoredPosition = pos;

        mainBackground.sizeDelta = new Vector2(totalPos, mainBackground.sizeDelta.y);
        endBackground.anchoredPosition = new Vector2(-totalPos, endBackground.anchoredPosition.y);
        RectTransform buttonTransform = toggleButton.GetComponent<RectTransform>();
        buttonTransform.anchoredPosition = new Vector2(buttonTransform.anchoredPosition.x - totalPos, buttonTransform.anchoredPosition.y);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!toggling)
        {
            if (eventData.pointerCurrentRaycast.gameObject == toggleButton)
            {
                if (open)
                {
                    StartCoroutine(Hide());
                }
                else
                {
                    StartCoroutine(Show());
                }
                return;
            }

            if (open)
            {
                if (eventData.pointerCurrentRaycast.gameObject == buttons[Spawner.Instance.buildings.Count])
                {
                    GameController.Instance.gameState = GameController.GameState.destroy;
                    return;
                }

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
    }

    IEnumerator Show()
    {
        toggling = true;
        Vector2 toPos = new Vector2(0, mainTransform.anchoredPosition.y);
        while (mainTransform.anchoredPosition != toPos)
        {
            mainTransform.anchoredPosition = Vector2.MoveTowards(mainTransform.anchoredPosition, toPos, toggleSpeed * Time.deltaTime);
            yield return null;
        }
        toggling = false;
        open = true;
    }
    IEnumerator Hide()
    {
        toggling = true;
        Vector2 toPos = new Vector2(totalPos + 42, mainTransform.anchoredPosition.y);
        while (mainTransform.anchoredPosition != toPos)
        {
            mainTransform.anchoredPosition = Vector2.MoveTowards(mainTransform.anchoredPosition, toPos, toggleSpeed * Time.deltaTime);
            yield return null;
        }
        toggling = false;
        open = false;
    }

}
