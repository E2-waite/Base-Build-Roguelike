using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : MonoSingleton<BuildingController>
{
    [System.Serializable]
    public class Building
    {
        public GameObject prefab;
        public int woodCost, stoneCost;
    }

    public List<Building> buildingTypes = new List<Building>();
    public Building selectedBuilding = null;
    
    public void Build(Grid.Tile tile)
    {
        if (tile.structure == null && selectedBuilding != null)
        {
            tile.structure = Instantiate(selectedBuilding.prefab, tile.tile.transform.position, Quaternion.identity);
            tile.structure.transform.parent = tile.tile.transform;
            tile.structure.GetComponent<Construct>().Setup(selectedBuilding.woodCost, selectedBuilding.stoneCost);
        }
    }
}
