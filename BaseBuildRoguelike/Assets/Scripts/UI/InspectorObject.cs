using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class InspectorObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Text title;
    public InspectorDetails currentDetails = null;
    public InspectorDetails storageDetails, constructionDetails, homeDetails, followerDetails, squadDetails;
    public bool mouseOver = false;
    public float toggleSpeed = 500f;
    public GameObject toggleButton;
    int startHeightBG, startHeightBox;
    RectTransform transformBG, transformBox, toggleTransform;
    float openX, closedX;
    bool open = true, toggling = false;
    private void Start()
    {
        Inspector.inspector = this;
        gameObject.SetActive(false);
        transformBG = transform.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        transformBox = transform.GetChild(0).GetChild(1).GetComponent<RectTransform>();
        toggleTransform = transform.GetChild(0).GetComponent<RectTransform>();
        startHeightBG = (int)transformBG.rect.height;
        startHeightBox = (int)transformBox.rect.height;
        openX = (int)transformBox.anchoredPosition.x;
        closedX = openX + 200;
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
        if (eventData.pointerCurrentRaycast.gameObject == toggleButton && !toggling)
        {
            if (open)
            {
                StartCoroutine(Hide());
            }
            else
            {
                StartCoroutine(Show());
            }
        }
    }
    public void Open(Interaction selected)
    {
        transformBG.sizeDelta = new Vector2(transformBG.rect.width, startHeightBG);
        transformBox.sizeDelta = new Vector2(transformBox.rect.width, startHeightBox);

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
        transformBG.sizeDelta = new Vector2(transformBG.rect.width, startHeightBG + resizeVal);
        transformBox.sizeDelta = new Vector2(transformBox.rect.width, startHeightBox + resizeVal);
    }

    IEnumerator Show()
    {
        toggling = true;
        while(toggleTransform.anchoredPosition.x != openX)
        {
            toggleTransform.anchoredPosition = Vector2.MoveTowards(toggleTransform.anchoredPosition, new Vector2(openX, toggleTransform.anchoredPosition.y), toggleSpeed * Time.deltaTime);
            yield return null;
        }
        toggling = false;
        open = true;
    }
    IEnumerator Hide()
    {
        toggling = true;
        while (toggleTransform.anchoredPosition.x != closedX)
        {
            toggleTransform.anchoredPosition = Vector2.MoveTowards(toggleTransform.anchoredPosition, new Vector2(closedX, toggleTransform.anchoredPosition.y), toggleSpeed * Time.deltaTime);
            yield return null;
        }
        toggling = false;
        open = false;
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
                return storageDetails;
            }
            else if (building is HomeBase)
            {
                return null;
            }

        }
        else
        {
            return null;
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
            title.text = "Squad";
            return squadDetails;
        }
    }
}

public abstract class InspectorDetails : MonoBehaviour
{
    public GameObject iconPrefab, textPrefab, healthBarPrefab;
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