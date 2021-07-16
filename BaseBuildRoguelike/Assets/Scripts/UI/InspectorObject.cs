using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class InspectorObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Text title;
    public InspectorDetails currentDetails = null;
    public InspectorDetails resourcesDetails, constructionDetails, homeDetails, followerDetails, squadDetails;
    public bool mouseOver = false;

    private void Start()
    {
        Inspector.inspector = this;
        gameObject.SetActive(false);
    }
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        mouseOver = false;
    }

    public void Open(Interaction selected)
    {

        title.text = selected.inspectorName;

        InspectorDetails newDetails = GetDetails(selected);

        if (newDetails == null)
        {
            // 'Interaction' Object does not have a corresponding inspector details object
            Debug.LogWarning("Object Incompatible With Inspector");
            gameObject.SetActive(false);
        }

        if (currentDetails != newDetails)
        {
            // Swap details menu
            if (currentDetails != null)
            {
                currentDetails.gameObject.SetActive(false);
            }
            newDetails.gameObject.SetActive(true);
        }

        currentDetails = newDetails;
        currentDetails.Reload(selected);
    }

    InspectorDetails GetDetails(Interaction selected)
    {
        if (selected is Building)
        {
            return BuildingDetails(selected as Building);
        }
        else if (selected is Follower)
        {
            return FollowerDetails(selected as Follower);
        }
        return null;
    }

    InspectorDetails BuildingDetails(Building building)
    {
        if (building.isConstructed)
        {
            if (building is ResourceStorage)
            {
                return resourcesDetails;
            }
            else if (building is HomeBase)
            {
                return homeDetails;
            }

        }
        else
        {
            return constructionDetails;
        }
        return null;
    }

    InspectorDetails FollowerDetails(Follower follower)
    {
        if (follower.squad == null)
        {
            return followerDetails;
        }
        else
        {
            return squadDetails;
        }
    }
}

public abstract class InspectorDetails : MonoBehaviour
{
    public GameObject iconPrefab, textPrefab;
    public virtual void Reload(Interaction selected) {}
}

public static class Inspector
{
    public static InspectorObject inspector;
    public static Interaction selected;

    public static void Enable(Interaction selected)
    {
        inspector.gameObject.SetActive(true);
        inspector.Open(selected);
    }

    public static void Disable()
    {
        inspector.gameObject.SetActive(false);
    }

    public static bool MouseOver()
    {
        if (inspector.gameObject.activeSelf && inspector.mouseOver)
        {
            return true;
        }
        return false;
    }
}