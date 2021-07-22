using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class TrainerInspector : InspectorDetails, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject[] buttons = new GameObject[3];
    public Image[] progressBars = new Image[3];
    public Trainer trainer;
    public override int Reload(Interaction selected)
    {
        trainer = selected as Trainer;

        for (int i = 0; i < 3; i++)
        {
            if (trainer.training[i] == null)
            {
                buttons[i].transform.GetChild(0).gameObject.SetActive(false);
                progressBars[i].gameObject.SetActive(false);
            }
            else
            {
                buttons[i].transform.GetChild(0).gameObject.SetActive(true);
                progressBars[i].gameObject.SetActive(true);
                Image icon = buttons[i].transform.GetChild(0).GetComponent<Image>();
                icon.sprite = Icons.Follower(Follower.Type.worker);
            }
        }
        return 70;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        for (int i = 0; i < 3; i++)
        { 
            if (eventData.pointerCurrentRaycast.gameObject == buttons[i] && trainer.training[i] == null && Followers.selected != null && Followers.selected is Worker)
            {
                progressBars[i].gameObject.SetActive(true);
                trainer.AddFollower(Followers.selected, i);
                Image icon = buttons[i].transform.GetChild(0).GetComponent<Image>();
                icon.sprite = Icons.Follower(Follower.Type.worker);
            }
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        for (int i = 0; i < 3; i++)
        {
            if (pointerEventData.pointerCurrentRaycast.gameObject == buttons[i] && trainer.training[i] == null && Followers.selected != null && Followers.selected is Worker)
            {
                buttons[i].transform.GetChild(0).gameObject.SetActive(true);
                buttons[i].transform.GetChild(0).GetComponent<Image>().sprite = Icons.Modify(0);
            }
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        for (int i = 0; i < 3; i++)
        {
            if (trainer.training[i] == null)
            {
                buttons[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                Image icon = buttons[i].transform.GetChild(0).GetComponent<Image>();
                icon.sprite = Icons.Follower(Follower.Type.worker);
            }
        }
    }

    private void Update()
    {
        if (trainer != null)
        {
            for (int i = 0; i < 3; i++)
            {
                if (trainer.training[i] == null)
                {
                    if (progressBars[i].gameObject.activeSelf)
                    {
                        progressBars[i].gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (trainer.training[i].time.Complete())
                    {
                        buttons[i].transform.GetChild(0).gameObject.SetActive(false);
                    }
                    if (progressBars[i].gameObject.activeSelf)
                    {
                        Cooldown time = trainer.training[i].time;
                        progressBars[i].fillAmount = (time.max - time.current) / time.max;
                    }
                }

            }
        }
    }
}
