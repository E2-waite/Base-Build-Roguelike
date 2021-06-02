using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Follower
{
    public enum State
    {
        idle,
        move,
        chopWood,
        mineStone,
        store,
        build,
        hunt
    }


    [Header("Worker Settings")]
    public State state = State.idle;
    public State lastState = State.idle;
    public float gatherTime = 2, buildTime = 1, hitTime = 0.5f;
    public Interaction target, lastTarget;
    public bool canGather = true, canBuild = true, canHit = true;

    private Inventory inventory;
    public void Setup()
    {
        inventory = GetComponent<Inventory>();
    }

    public void Direct(Vector2 pos, GameObject obj)
    {
        StopAllCoroutines();
        canGather = true;
        canBuild = true;
        marker.transform.position = pos;
        lastState = State.idle;
        if (obj != null)
        {
            Debug.Log("Target: " + obj.name);
            target = obj.GetComponent<Interaction>();

            marker.transform.position = obj.transform.position;


            if (target.type == Interaction.Type.resource)
            {
                if (target.resource.type == Resource.Type.wood)
                {
                    state = State.chopWood;
                }
                else if (target.resource.type == Resource.Type.stone)
                {
                    state = State.mineStone;
                }

                if (inventory.AtCapacity())
                {
                    FindStorage();
                }
            }
            else if (target.type == Interaction.Type.building)
            {
                if (target.building.isConstructed)
                {
                    if (target.building.type == Building.Type.storage)
                    {
                        state = State.store;
                    }
                }
                else
                {
                    state = State.build;
                }
            }
            else if (target.type == Interaction.Type.creature)
            {
                state = State.hunt;

                if (inventory.AtCapacity())
                {
                    FindStorage();
                }
            }

            lastTarget = target;
            lastState = state;
        }
        else
        {
            target = null;
            state = State.move;
        }
    }

    private void Update()
    {
        if (state == State.move)
        {
            if (transform.position == marker.transform.position)
            {
                state = State.idle;
            }
            else
            {
                Move(marker.transform.position);
            }
        }
        else
        {
            if (target == null)
            {
                if ((state == State.chopWood || state == State.mineStone || state == State.hunt) && !inventory.AtCapacity())
                {
                    target = FindResource();
                }
                else
                {
                    state = State.idle;
                }
            }
            else
            {
                if (Vector2.Distance(transform.position, target.transform.position) <= targetDist)
                {
                    if ((state == State.chopWood || state == State.mineStone) && canGather)
                    {
                        StartCoroutine(GatherRoutine());
                    }
                    else if (state == State.store)
                    {
                        Store();
                    }
                    else if (state == State.build && canBuild)
                    {
                        StartCoroutine(BuildRoutine());
                    }
                    else if (state == State.hunt && canHit)
                    {
                        StartCoroutine(HitRoutine());
                    }
                }
                else
                {
                    Move(target.transform.position);
                }
            }
        }
    }

    void Store()
    {
        ResourceStorage storage = (ResourceStorage)target.building;
        if (storage.storageType == Resource.Type.wood)
        {
            storage.Store(ref inventory.wood);
        }
        else if (storage.storageType == Resource.Type.stone)
        {
            storage.Store(ref inventory.stone);
        }
        else if (storage.storageType == Resource.Type.food)
        {
            storage.Store(ref inventory.food);
        }

        if (!FindStorage() && (lastState == State.chopWood || lastState == State.mineStone || lastState == State.hunt))
        {
            state = lastState;
            if (lastTarget != null)
            {
                target = lastTarget;
            }
            else
            {
                target = FindResource();
            }
        }
    }

    bool FindStorage()
    {
        target = ClosestStorage();
        if (target == null)
        {
            Debug.Log("Either no resource storage available, or nothing to store...");
            return false;
        }
        else
        {
            state = State.store; 
            return true;
        }
    }

    Interaction ClosestStorage()
    {
        // Returns closest resource storage
        float closestDist = 10000;
        ResourceStorage closestStorage = null;
        List<ResourceStorage> storageBuildings = new List<ResourceStorage>();

        // Only checks resources that are in the follower's inventory
        if (inventory.wood > 0)
        {
            storageBuildings.AddRange(BuildingController.Instance.woodPiles);
        }
        if (inventory.stone > 0)
        {
            storageBuildings.AddRange(BuildingController.Instance.stonePiles);
        }
        if (inventory.food > 0)
        {
            storageBuildings.AddRange(BuildingController.Instance.foodPiles);
        }

        foreach (ResourceStorage storage in storageBuildings)
        {
            float dist = Vector2.Distance(transform.position, storage.transform.position);
            if (dist < closestDist && storage.currentStorage < storage.maxStorage)
            {
                closestDist = dist;
                closestStorage = storage;
            }
        }

        Interaction interaction = null;

        if (closestStorage != null)
        {
            interaction = closestStorage.interaction;
        }

        return interaction;
    }

    Interaction FindResource()
    {
        // Find closest resource of the correct type, based on the previous resource
        List<Interaction> resources = new List<Interaction>();
        if (lastState == State.chopWood)
        {
            resources = GameController.Instance.grid.trees;
        }

        if (lastState == State.mineStone)
        {
            resources = GameController.Instance.grid.stones;
        }

        if (lastState == State.hunt)
        {
            resources = CreatureController.Instance.creatures;
        }


        Interaction closestResource = null;
        float closestDist = 999999;
        foreach(Interaction resource in resources)
        {
            if (resource != null)
            {
                float dist = Vector2.Distance(marker.transform.position, resource.transform.position);
                if (dist < closestDist)
                {
                    closestResource = resource;
                    closestDist = dist;
                }
            }
        }

        state = lastState;

        return closestResource;
    }

    IEnumerator GatherRoutine()
    {
        // Gather resource in range
        canGather = false;
        yield return new WaitForSeconds(gatherTime);

        if (target != null)
        {
            target.resource.Gather(inventory);
        }

        if (inventory.AtCapacity())
        {
            FindStorage();
        }

        canGather = true;
    }

    IEnumerator BuildRoutine()
    {
        // Construct building in range
        canBuild = false;
        yield return new WaitForSeconds(buildTime);

        if (target != null)
        {
            target.building.construct.Build();
        }

        canBuild = true;
    }

    IEnumerator HitRoutine()
    {
        // Hit target in range
        canHit = false;
        yield return new WaitForSeconds(hitTime);

        if (target != null)
        {
            if (state == State.hunt && target.creature.Hit(hitDamage))
            {
                // If target creature dies, gather food
                target.creature.GatherFood(inventory);

                // Then check if inventory is full, if so stores resources
                if (inventory.AtCapacity())
                {
                    FindStorage();
                }
            }
        }

        canHit = true;
    }
}
