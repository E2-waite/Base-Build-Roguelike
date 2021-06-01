using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ResourceDetails : MonoBehaviour
{
    public Text resourcesText;
    public Image resourceIcon;
    public Sprite woodIcon, stoneIcon, foodIcon;

    public void Reload(Building building)
    {
        resourcesText.text = building.storage.currentStorage.ToString() + "/" + building.storage.maxStorage.ToString();

        if (building.storage.type == Resource.Type.wood)
        {
            resourceIcon.sprite = woodIcon;
        }
        else if (building.storage.type == Resource.Type.stone)
        {
            resourceIcon.sprite = stoneIcon;
        }
        else if (building.storage.type == Resource.Type.food)
        {
            resourceIcon.sprite = foodIcon;
        }
    }
}
