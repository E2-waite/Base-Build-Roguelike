using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Follower
{
    enum State : int
    {
        idle = 0,
        move = 1,
        chopWood = 2,
        mineStone = 3,
        store = 4,
        build = 5,
        hunt = 6, 
    }
    [Header("Worker Settings")]
    public int lastState = 0;
    public float gatherTime = 2, buildTime = 1, hitTime = 0.5f;
    public Interaction lastTarget;

    public Inventory inventory = new Inventory();
    public override void Setup()
    {
    }

    public override void Direct(Vector2 pos, Interaction obj)
    {
        if (interactRoutine != null)
        {
            StopCoroutine(interactRoutine);
            interactRoutine = null;
        }

        marker.transform.position = pos;
        Pathfinding.FindPath(ref path, transform.position, new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)), 1);
        lastState = (int)State.idle;
        if (obj != null)
        {
            target = new Target(obj);
            marker.transform.position = obj.transform.position;

            if (target.interact is Resource)
            {
                Resource resource = target.interact as Resource;
                if (resource.type == Resource.Type.wood)
                {
                    state = (int)State.chopWood;
                }
                else if (resource.type == Resource.Type.stone)
                {
                    state = (int)State.mineStone;
                }

                if (inventory.AtCapacity())
                {
                    FindStorage();
                }
            }
            else if (target.interact is Building)
            {
                Building building = target.interact as Building;
                if (building.isConstructed)
                {
                    if (building is ResourceStorage)
                    {
                        state = (int)State.store;
                    }
                }
                else
                {
                    state = (int)State.build;
                }
            }
            else if (target.interact is Creature)
            {
                state = (int)State.hunt;

                if (inventory.AtCapacity())
                {
                    FindStorage();
                }
            }

            lastTarget = target.interact;
            lastState = state;
        }
        else
        {
            target = new Target();
            state = (int)State.move;
        }
    }

    private void Update()
    {
        TickEffects();
        if (state == (int)State.move)
        {
            if (transform.position == marker.transform.position)
            {
                state = (int)State.idle;
            }
            else
            {
                Move();
            }
        }
        else
        {
            if (target.interact == null)
            {
                if ((state == (int)State.chopWood || state == (int)State.mineStone || state == (int)State.hunt) && !inventory.AtCapacity())
                {
                    target = new Target(FindResource());

                    if (Pathfinding.FindPath(ref path, transform.position, target.Position2D(), 1))
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
                    state = (int)State.idle;
                }
            }
            else
            {
                if (Vector2.Distance(transform.position, target.Position()) <= targetDist)
                {
                    if ((state == (int)State.chopWood || state == (int)State.mineStone) && interactRoutine == null)
                    {
                        interactRoutine = StartCoroutine(GatherRoutine());
                    }
                    else if (state == (int)State.store)
                    {
                        Store();
                    }
                    else if (state == (int)State.build && interactRoutine == null)
                    {
                        interactRoutine = StartCoroutine(BuildRoutine());
                    }
                    else if (state == (int)State.hunt && interactRoutine == null)
                    {
                        interactRoutine = StartCoroutine(HitRoutine());
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
        ResourceStorage storage = target.interact as ResourceStorage;

        storage.Store(ref inventory.resources[(int)storage.storageType]);

        if (!FindStorage() && (lastState == (int)State.chopWood || lastState == (int)State.mineStone || lastState == (int)State.hunt))
        {
            state = lastState;
            if (lastTarget != null)
            {
                target = new Target(lastTarget);
            }
            else
            {
                target = new Target(FindResource());
            }

            if (target.interact != null)
            {
                Pathfinding.FindPath(ref path, transform.position, target.Position2D(), 1);
            }
        }
    }

    bool FindStorage()
    {
        target = new Target(ClosestStorage());

        if (target.interact == null)
        {
            Debug.Log("Either no resource storage available, or nothing to store...");
            return false;
        }
        else
        {
            state = (int)State.store;
            Pathfinding.FindPath(ref path, transform.position, target.Position2D(), 1);
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
        for (int i = 0; i < Resources.NUM; i++)
        {
            if (inventory.resources[i] > 0)
            {
                storageBuildings.AddRange(Buildings.storages[i]);
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
        if (lastState == (int)State.chopWood)
        {
            resources = Resources.trees;
        }

        if (lastState == (int)State.mineStone)
        {
            resources = Resources.stones;
        }

        if (lastState == (int)State.hunt)
        {
            resources = Creatures.creatures;
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
        yield return new WaitForSeconds(gatherTime);

        if (target.interact != null)
        {
            Resource resource = target.interact as Resource;
            resource.Gather(inventory);
        }

        if (inventory.AtCapacity())
        {
            FindStorage();
        }

        interactRoutine = null;
    }

    IEnumerator BuildRoutine()
    {
        // Construct building in range
        yield return new WaitForSeconds(buildTime);

        if (target.interact != null)
        {
            Building building = target.interact as Building;
            building.construct.Build();
        }
        interactRoutine = null;
    }

    IEnumerator HitRoutine()
    {
        // Hit target in range
        yield return new WaitForSeconds(hitTime);

        if (target.interact != null)
        {
            Creature creature = target.interact as Creature;
            if (state == (int)State.hunt && creature.Hit(hitDamage))
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
        interactRoutine = null;
    }
}
