using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureController : MonoSingleton<CreatureController>
{
    public GameObject rabbitPrefab;
    public List<Interaction> creatures = new List<Interaction>();
    public int creatureScale = 10, maxCreatures;

    public void SpawnCreatures(int mapSize)
    {
        maxCreatures = creatureScale * (mapSize / 10);
        for (int i = 0; i < maxCreatures; i++)
        {
            Vector3 creaturePos = new Vector3((int)(Random.Range(0, mapSize)), (int)(Random.Range(0, mapSize)), 0);
            GameObject creature = Instantiate(rabbitPrefab, creaturePos, Quaternion.identity);
            creatures.Add(creature.GetComponent<Interaction>());
        }
    }
}
