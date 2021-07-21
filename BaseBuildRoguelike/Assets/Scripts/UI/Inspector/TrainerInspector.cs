using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class TrainerInspector : InspectorDetails, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject[] icons = new GameObject[3];
    public Trainer trainer;
    public override int Reload(Interaction selected)
    {
        trainer = selected as Trainer;

        //Sprite sprite = null;
        //if (trainer is Barracks)
        //{
        //    sprite = Icons.Follower(Follower.Type.soldier);
        //}
        //else if (trainer is ArcheryRange)
        //{
        //    sprite = Icons.Follower(Follower.Type.archer);
        //}
        //else if (trainer is Temple)
        //{
        //    sprite = Icons.Follower(Follower.Type.priest);
        //}
        //int resizeVal = 0;
        //Vector2 iconPos = startIcon, progressBarPos = startInfo;
        //for (int i = 0; i < 3; i++)
        //{
        //    icons[i].GetComponent<Image>().sprite = sprite;
        //    iconPos.y -= offset;
        //    progressBarPos.y -= offset;
        //    resizeVal = i * offset;
        //}

        return 90;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        for (int i = 0; i < 3; i++)
        { 
            if (eventData.pointerCurrentRaycast.gameObject == icons[i] && trainer.training[i] == null && Followers.selected != null && Followers.selected is Worker)
            {
                trainer.AddFollower(Followers.selected, i);
            }
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        for (int i = 0; i < 3; i++)
        {
            if (pointerEventData.pointerCurrentRaycast.gameObject == icons[i] && trainer.training[i] == null && Followers.selected != null && Followers.selected is Worker)
            {
                icons[i].GetComponent<Image>().sprite = Icons.Modify(0);
            }
        }
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        for (int i = 0; i < 3; i++)
        {

                Image icon = icons[i].GetComponent<Image>();
                icon.sprite = Icons.Follower(Follower.Type.worker);
        }
    }
}
