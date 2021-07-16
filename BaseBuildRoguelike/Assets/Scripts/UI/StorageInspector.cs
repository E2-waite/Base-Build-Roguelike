using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StorageInspector : InspectorDetails
{
    public Text resourcesText;
    public Image resourceIcon;
    public override int Reload(Interaction selected)
    {
        ResourceStorage storage = selected as ResourceStorage;

        resourcesText.text = storage.currentStorage.ToString() + "/" + storage.maxStorage.ToString();

        resourceIcon.sprite = Icons.Resource(storage.storageType);
        return 0;
    }
}
