using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ResourceDetails : MonoBehaviour
{
    public Text resourcesText;
    public Image resourceIcon;
    public Sprite woodIcon, stoneIcon, foodIcon;

    public void Reload(ResourceStorage storage)
    {
        resourcesText.text = storage.currentStorage.ToString() + "/" + storage.maxStorage.ToString();

        if (storage.storageType == Resource.Type.wood)
        {
            resourceIcon.sprite = woodIcon;
        }
        else if (storage.storageType == Resource.Type.stone)
        {
            resourceIcon.sprite = stoneIcon;
        }
        else if (storage.storageType == Resource.Type.food)
        {
            resourceIcon.sprite = foodIcon;
        }
    }
}
