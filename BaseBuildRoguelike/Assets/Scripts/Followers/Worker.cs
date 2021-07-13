using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Follower
{
    [Header("Worker Settings")]
    public State lastState = State.idle;
    public float gatherTime = 2, buildTime = 1, hitTime = 0.5f;
    public Interaction lastTarget;
    public bool canGather = true, canBuild = true, canHit = true;

    private Inventory inventory;
    public override void Setup()
    {
        inventory = GetComponent<Inventory>();
    }

    public override void Direct(Vector2 pos, Interaction obj)
    {
        StopAllCoroutines();
        canGather = true;
        canBuild = true;
        canHit = true;
        marker.transform.position = pos;
        Pathfinding.FindPath(ref path, transform.position, pos);
        lastState = State.idle;
        if (obj != null)
        {
            Debug.Log("Target: " + obj.name);
            target = obj;

            marker.transform.position = obj.transform.position;


            if (target is Resource)
            {
                Resource resource = target as Resource;
                if (resource.type == Resource.Type.wood)
                {
                    state = State.chopWood;
                }
                else if (resource.type == Resource.Type.stone)
                {
                    state = State.mineStone;
                }

                if (inventory.AtCapacity())
                {
                    FindStorage();
                }
            }
            else if (target is Building)
            {
                Building building = target as Building;
                if (building.isConstructed)
                {
                    if (building is ResourceStorage)
                    {
                        state = State.store;
                    }
                }
                else
                {
                    state = State.build;
                }
            }
            else if (target is Creature)
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
                Move();
            }
        }
        else
        {
            if (target == null)
            {
                if ((state == State.chopWood || state == State.mineStone || state == State.hunt) && !inventory.AtCapacity())
                {
                    target = FindResource();
                    Debug.Log("Start: " + transform.position.ToString() + " End: " + target.transform.position.ToString());
                    if (Pathfinding.FindPath(ref path, transform.position, target.transform.position))
                    {
                        Debug.Log("Path Found");
                    }
                    else
                    {
                        Debug.Log("No Path");
                    }
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
                    Move();
                }
            }
        }
    }

    void Store()
    {
        ResourceStorage storage = target as ResourceStorage;

        storage.Store(ref inventory.resources[(int)storage.storageType]);

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

            if (target != null)
            {
                Pathfinding.FindPath(ref path, transform.position, target.transform.position);
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
            Pathfinding.FindPath(ref path, transform.position, target.transform.position);
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
        for (int i = 0; i < Consts.NUM_RESOURCES; i++)
        {
            if (inventory.resources[i] > 0)
            {
                storageBuildings.AddRange(BuildingController.Instance.storages[i]);
            }
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

        return closestStorage;
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
            Resource resource = target as Resource;
            resource.Gather(inventory);
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
            Building building = target as Building;
            building.construct.Build();
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
            Creature creature = target as Creature;
            if (state == State.hunt && creature.Hit(hitDamage))
            {
                // If target creature dies, gather food
                creature.GatherFood(inventory);

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
