using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class InspectorObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Text title;
    public InspectorDetails currentDetails = null;
    public InspectorDetails followerDetails, squadDetails, buildingDetails;
    public bool mouseOver = false;
    public float toggleSpeed = 500f;
    int startMiddleHeight, startBottomPos;
    public RectTransform topRect, middleRect, bottomRect;
    float openX, closedX;
    bool open = true, toggling = false;
    private void Start()
    {
        Inspector.inspector = this;
        gameObject.SetActive(false);
        startMiddleHeight = (int)middleRect.rect.height;
        startBottomPos = (int)bottomRect.anchoredPosition.y;
    }
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        mouseOver = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
    }
    public void Open(Interaction selected)
    {
        topRect.sizeDelta = new Vector2(topRect.rect.width, startMiddleHeight);
        middleRect.sizeDelta = new Vector2(middleRect.rect.width, startBottomPos);

        title.text = selected.inspectorName;

        InspectorDetails newDetails = GetDetails(selected);

        if (newDetails == null)
        {
            // 'Interaction' Object does not have a corresponding inspector details object
            Debug.LogWarning("Object Incompatible With Inspector");
            gameObject.SetActive(false);
            return;
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
        int resizeVal = currentDetails.Reload(selected);
        middleRect.sizeDelta = new Vector2(topRect.rect.width, startMiddleHeight + resizeVal);
        bottomRect.anchoredPosition = new Vector2(bottomRect.anchoredPosition.x, startBottomPos - resizeVal);
    }

    InspectorDetails GetDetails(Interaction selected)
    {
        if (selected is Building)
        {
            return buildingDetails;
        }
        else if (selected is Follower)
        {
            Follower follower = selected as Follower;

            if (follower.squad == null)
            {
                return followerDetails;
            }
            else
            {
                title.text = "Squad";
                return squadDetails;
            }
        }
        return null;
    }
}

public abstract class InspectorDetails : MonoBehaviour
{
    public GameObject iconPrefab, textPrefab, healthBarPrefab;
    protected Vector2 startIcon = new Vector2(-195, 34.5f), startInfo = new Vector2(-165, 26);
    protected int offset = 35;
    public virtual int Reload(Interaction selected) { return 0; }
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