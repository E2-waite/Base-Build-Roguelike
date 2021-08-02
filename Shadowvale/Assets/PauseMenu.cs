using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject playButton, exitButton;
    public Color textColour, rolloverColour;

    public void OnPointerClick(PointerEventData eventData)
    {   
        if (eventData.pointerCurrentRaycast.gameObject == playButton)
        {
            GameController.Instance.Unpause();
        }
        else if (eventData.pointerCurrentRaycast.gameObject == exitButton)
        {
            GameController.Instance.Exit();
        }
    }

   public void OnPointerEnter(PointerEventData pointerEventData)
    {

        if (pointerEventData.pointerCurrentRaycast.gameObject == playButton)
        {
            playButton.transform.GetChild(0).GetComponent<Text>().color = rolloverColour;
        }
        if (pointerEventData.pointerCurrentRaycast.gameObject == exitButton)
        {
            exitButton.transform.GetChild(0).GetComponent<Text>().color = rolloverColour;
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        playButton.transform.GetChild(0).GetComponent<Text>().color = textColour;
        exitButton.transform.GetChild(0).GetComponent<Text>().color = textColour;
    }
}
