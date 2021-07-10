using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoSingleton<BuildingController>
{
    [System.Serializable]
    public class BuildingTemplate
    {
        public string name;
        public GameObject prefab;
        public Sprite sprite;
    }
    public GameObject firepitPrefab;
    public List<BuildingTemplate> buildingTypes = new List<BuildingTemplate>();
    public BuildingTemplate selectedTemplate = null;

    public Building selected;
    public List<ResourceStorage>[] storages = new List<ResourceStorage>[Consts.NUM_RESOURCES];
    public HomeBase homeBase;
    public Wall[,] walls;
    public Inspector inspector;
    private void Start()
    {
        walls = new Wall[GameController.Instance.grid.mapSize, GameController.Instance.grid.mapSize];
        for (int i = 0; i < Consts.NUM_RESOURCES; i++)
        {
            storages[i] = new List<ResourceStorage>();
        }
    }

    public void SpawnHome(Tile tile)
    {
        GameObject building = Instantiate(firepitPrefab, tile.transform.position, Quaternion.identity);
        tile.structure = building.GetComponent<Interaction>();
        homeBase = tile.structure as HomeBase;
    }

    public void Build(Tile tile)
    {
        if (tile != null && tile.structure == null && selectedTemplate != null)
        {
            GameObject building = Instantiate(selectedTemplate.prefab, tile.transform.position, Quaternion.identity);
            tile.structure = building.GetComponent<Interaction>();
            tile.structure.transform.parent = tile.transform;
        }
    }

    public void Select(GameObject obj)
    {
        if (obj != null)
        {
            selected = obj.GetComponent<Building>();
            selected.selected = true;
            inspector.gameObject.SetActive(true);
            inspector.Reload(selected);
        }
    }

    public void Deselect()
    {
        if (selected != null)
        {
            selected.selected = false;
            selected = null;
            inspector.gameObject.SetActive(false);
        }
    }

    public bool UseResource(Resource.Type type, int val)
    {
        foreach (ResourceStorage storage in storages[(int)type])
        {
            if (storage.Withdraw(ref val))
            {
                return true;
            }
        }
        return false;
    }
}
