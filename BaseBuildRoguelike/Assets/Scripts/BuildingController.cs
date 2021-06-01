using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoSingleton<BuildingController>
{
    [System.Serializable]
    public class BuildingTemplate
    {
        public GameObject prefab;
        public int woodCost, stoneCost;
    }

    public List<BuildingTemplate> buildingTypes = new List<BuildingTemplate>();
    public BuildingTemplate selectedBuilding = null;

    public List<Building> woodPiles = new List<Building>();
    public List<Building> stonePiles = new List<Building>();
    public List<Building> foodPiles = new List<Building>();
    public Wall[,] walls;

    private void Start()
    {
        walls = new Wall[GameController.Instance.grid.mapSize, GameController.Instance.grid.mapSize];
    }

    public void Build(Grid.Tile tile)
    {
        if (tile.structure == null && selectedBuilding != null)
        {
            tile.structure = Instantiate(selectedBuilding.prefab, tile.tile.transform.position, Quaternion.identity);
            tile.structure.transform.parent = tile.tile.transform;
            tile.structure.GetComponent<Construct>().Setup(selectedBuilding.woodCost, selectedBuilding.stoneCost);
        }
    }

    public bool UseResource(Resource.Type type, int val)
    {
        List<Building> resourceStorage = new List<Building>();

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

        foreach (Building building in resourceStorage)
        {
            if (building.storage.Withdraw(ref val))
            {
                return true;
            }
        }
        return false;
    }
}
