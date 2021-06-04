using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoSingleton<BuildingController>
{
    [System.Serializable]
    public class BuildingTemplate
    {
        public GameObject prefab;
    }
    public GameObject firepitPrefab;
    public List<BuildingTemplate> buildingTypes = new List<BuildingTemplate>();
    public BuildingTemplate selectedTemplate = null;

    public Building selected;

    public List<ResourceStorage> woodPiles = new List<ResourceStorage>();
    public List<ResourceStorage> stonePiles = new List<ResourceStorage>();
    public List<ResourceStorage> foodPiles = new List<ResourceStorage>();
    public HomeBase homeBase;
    public Wall[,] walls;
    public Inspector inspector;
    private void Start()
    {
        walls = new Wall[GameController.Instance.grid.mapSize, GameController.Instance.grid.mapSize];
    }

    public void SpawnHome(Grid.Tile tile)
    {
        tile.structure = Instantiate(firepitPrefab, tile.tile.transform.position, Quaternion.identity);
        homeBase = tile.structure.GetComponent<HomeBase>();
    }

    public void Build(Grid.Tile tile)
    {
        if (tile.structure == null && selectedTemplate != null)
        {
            tile.structure = Instantiate(selectedTemplate.prefab, tile.tile.transform.position, Quaternion.identity);
            tile.structure.transform.parent = tile.tile.transform;
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
        List<ResourceStorage> resourceStorage = new List<ResourceStorage>();

        if (type == Resource.Type.wood)
        {
            resourceStorage = woodPiles;
        }
        else if (type == Resource.Type.stone)
        {
            resourceStorage = stonePiles;
        }
        else if (type == Resource.Type.food)
        {
            resourceStorage = foodPiles;
        }

        foreach (ResourceStorage storage in resourceStorage)
        {
            if (storage.Withdraw(ref val))
            {
                return true;
            }
        }
        return false;
    }
}
