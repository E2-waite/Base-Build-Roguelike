using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStorage : Building
{
    [Header("Storage Settings")]
    public Resource.Type storageType;
    public int maxStorage = 100, currentStorage = 0;
    public List<Sprite> stages = new List<Sprite>();
    SpriteRenderer rend;

    public void Setup()
    {
        rend = GetComponent<SpriteRenderer>();
        rend.sprite = stages[0];

        if (storageType == Resource.Type.wood)
        {
            BuildingController.Instance.woodPiles.Add(this);
        }
        else if (storageType == Resource.Type.stone)
        {
            BuildingController.Instance.stonePiles.Add(this);
        }
        else if (storageType == Resource.Type.food)
        {
            BuildingController.Instance.foodPiles.Add(this);
        }

        GameController.Instance.AdjustResources(storageType, 0, maxStorage);
    }

    public void Store(ref int val)
    {
        int toStore = val;
        if (currentStorage + val > maxStorage)
        {
            toStore = maxStorage - currentStorage;
        }

        currentStorage += toStore;
        val -= toStore;


        GameController.Instance.AdjustResources(storageType, toStore, 0);

        rend.sprite = stages[(int)Mathf.Ceil((currentStorage * (stages.Count - 1)) / maxStorage)];
        ReloadInspector();
    }

    public bool Withdraw(ref int remaining)
    {
        if (currentStorage >= remaining)
        {
            currentStorage -= remaining;
            GameController.Instance.AdjustResources(storageType, -remaining, 0);
            remaining = 0;

            rend.sprite = stages[(int)Mathf.Ceil((currentStorage * (stages.Count - 1)) / maxStorage)];
            ReloadInspector();
            return true;
        }
        else
        {
            remaining -= currentStorage;
            GameController.Instance.AdjustResources(storageType, -currentStorage, 0);
            currentStorage = 0;

            rend.sprite = stages[(int)Mathf.Ceil((currentStorage * (stages.Count - 1)) / maxStorage)];
            ReloadInspector();
            return false;
        }
    }
}
