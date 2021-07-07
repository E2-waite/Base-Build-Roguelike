using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Inspector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum Type
    {
        none,
        resources,
        construction,
        home
    }
    public Type currentType;
    public Text title;

    public ResourceDetails resources;
    public ConstructionDetails construction;
    public HomeDetails home;
    public bool mouseOver = false;
    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        mouseOver = false;
    }

    public void Reload(Building building)
    {
        title.text = building.name;

        Type thisType = Type.none;


        if (building.isConstructed)
        {
            if (building is ResourceStorage)
            {
                thisType = Type.resources;
            }
            else if (building is HomeBase)
            {
                thisType = Type.home;
            }
        }
        else
        {
            thisType = Type.construction;
        }

        // Only swaps activated details gameobject if not the same type
        if (thisType != currentType)
        {
            SwapDetails(thisType);
        }

        ReloadDetails(building);
    }



    void ReloadDetails(Building building)
    {
        if (currentType == Type.resources)
        {
            resources.Reload(building as ResourceStorage);
        }
        else if (currentType == Type.construction)
        {
            construction.Reload(building);
        }
        else if (currentType == Type.home)
        {
            home.Reload(building);
        }
    }

    void SwapDetails(Type type)
    {
        if (type == Type.resources)
        {
            resources.gameObject.SetActive(true);
            construction.gameObject.SetActive(false);
            home.gameObject.SetActive(false);
        }
        else if (type == Type.construction)
        {
            resources.gameObject.SetActive(false);
            construction.gameObject.SetActive(true);
            home.gameObject.SetActive(false);
        }
        else if (type == Type.home)
        {
            resources.gameObject.SetActive(false);
            construction.gameObject.SetActive(true);
            home.gameObject.SetActive(true);
        }
        else
        {
            resources.gameObject.SetActive(false);
            construction.gameObject.SetActive(false);
            home.gameObject.SetActive(false);
        }
        currentType = type;
    }

    private void OnDestroy()
    {
        Debug.Log("DESTROYED");
    }
}
