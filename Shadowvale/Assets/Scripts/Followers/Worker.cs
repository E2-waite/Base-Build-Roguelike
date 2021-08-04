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
        repair = 7
    }
    [Header("Worker Settings")]
    public float gatherTime = 2, buildTime = 1, hitTime = 0.5f, repairTime = 1;

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

        // Clear the action list, before assigning a new action 
        actions = new List<Action>();
        actions.Add(new Action(new Target(), (int)State.idle));
        int state = 0;
        Target target = new Target();


        marker.transform.position = pos;

        if (obj != null)
        {
            target = new Target(obj);
            marker.transform.position = obj.transform.position;

            if (target.interact is Resource)
            {
                Resource resource = target.interact as Resource;
                if (resource.type == Resource.Type.wood)
                {
                    actions.Add(new Action(target, (int)State.chopWood));
                }
                else if (resource.type == Resource.Type.stone)
                {
                    actions.Add(new Action(target, (int)State.mineStone));
                }

                if (inventory.AtCapacity())
                {
                    if (!FindStorage())
                    {
                        //Idle for now, but needs to be able to go back to store state as soon as storage is built
                        Idle();
                    }
                    return;
                }
            }
            else if (target.interact is Building)
            {
                Building building = target.interact as Building;
                if (building.isConstructed)
                {
                    if (building.repair < building.maxRepair)
                    {
                        actions.Add(new Action(target, (int)State.repair));
                    }

                    if (building is ResourceStorage)
                    {
                        actions.Add(new Action(target, (int)State.store));
                    }
                }
                else
                {
                    actions.Add(new Action(target, (int)State.build));
                }
            }
            else if (target.interact is Creature)
            {
                actions.Add(new Action(target, (int)State.hunt));

                if (inventory.AtCapacity())
                {
                    if (!FindStorage())
                    {
                        //Idle for now, but needs to be able to go back to store state as soon as storage is built
                        Idle();
                    }
                    return;
                }
            }

            Pathfinding.FindPath(ref path, currentPos, target.Position2D(), 1);
        }
        else
        {
            actions.Add(new Action(new Target(), (int)State.move));
            Pathfinding.FindPath(ref path, currentPos, new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y)));
        }
        currentAction = actions[actions.Count - 1];
    }

    private void Update()
    {
        TickEffects();
        if (currentAction.state == (int)State.move)
        {
            if (transform.position == marker.transform.position)
            {
                Idle();
            }
            else
            {
                Move();
            }
        }
        else
        {
            if (currentAction.target.interact == null)
            {
                if ((currentAction.state == (int)State.chopWood || currentAction.state == (int)State.mineStone || currentAction.state == (int)State.hunt) && !inventory.AtCapacity())
                {
                    FindResource((State)currentAction.state);
                }
                else
                {
                    Idle();
                }
            }
            else
            {
                if (Vector2.Distance(transform.position, currentAction.target.Position()) <= targetDist)
                {
                    if ((currentAction.state == (int)State.chopWood || currentAction.state == (int)State.mineStone) && interactRoutine == null)
                    {
                        interactRoutine = StartCoroutine(GatherRoutine());
                    }
                    else if (currentAction.state == (int)State.store)
                    {
                        Store();
                    }
                    else if (currentAction.state == (int)State.build && interactRoutine == null)
                    {
                        interactRoutine = StartCoroutine(BuildRoutine());
                    }
                    else if (currentAction.state == (int)State.hunt && interactRoutine == null)
                    {
                        interactRoutine = StartCoroutine(HitRoutine());
                    }
                    else if (currentAction.state == (int)State.repair && interactRoutine == null)
                    {
                        interactRoutine = StartCoroutine(RepairRoutine());
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
        ResourceStorage storage = currentAction.target.interact as ResourceStorage;

        storage.Store(ref inventory.resources[(int)storage.storageType]);

        if (!FindStorage())
        {
            for (int i = actions.Count - 1; i >= 0; i--)
            {
                if (actions[i].state != (int)State.store)
                {
                    if (actions[i].target.interact != null)
                    {
                        currentAction = actions[i];
                        Pathfinding.FindPath(ref path, currentPos, currentAction.target.Position2D(), 1);
                    }
                    else
                    {
                        FindResource((State)actions[i].state);
                    }
                    return;
                }
                else
                {
                    actions.RemoveAt(i);
                }
            }
            Idle();
        }
    }

    bool FindStorage()
    {
        Interaction storage = ClosestStorage();

        if (storage == null)
        {
            Debug.Log("Either no resource storage available, or nothing to store...");
            return false;
        }
        else
        {
            actions.Add(new Action(new Target(storage), (int)State.store));
            currentAction = actions[actions.Count - 1];
            Pathfinding.FindPath(ref path, currentPos, currentAction.target.Position2D(), 1);
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

    void FindResource(State state)
    {
        // Find closest resource of the correct type, based on the previous resource
        List<Interaction> resources = new List<Interaction>();

        if (state == State.chopWood)
        {
            resources = Resources.trees;
        }

        if (state == State.mineStone)
        {
            resources = Resources.stones;
        }

        if (state == State.hunt)
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

        currentAction = new Action(new Target(closestResource), (int)state);
        actions.Add(currentAction);
        Pathfinding.FindPath(ref path, currentPos, currentAction.target.Position2D(), 1);
    }

    IEnumerator GatherRoutine()
    {
        // Gather resource in range
        yield return new WaitForSeconds(gatherTime);

        if (currentAction.target.interact != null)
        {
            Resource resource = currentAction.target.interact as Resource;
            resource.Gather(inventory);
        }

        if (inventory.AtCapacity() && !FindStorage())
        {
            //Idle for now, but needs to be able to go back to store state as soon as storage is built
            Idle();
        }

        interactRoutine = null;
    }

    IEnumerator BuildRoutine()
    {
        // Construct building in range
        yield return new WaitForSeconds(buildTime);

        if (currentAction.target.interact != null)
        {
            Building building = currentAction.target.interact as Building;
            building.construct.Build();
        }
        interactRoutine = null;
    }

    IEnumerator HitRoutine()
    {
        // Hit target in range
        yield return new WaitForSeconds(hitTime);

        if (currentAction.target.interact != null)
        {
            Creature creature = currentAction.target.interact as Creature;
            if (currentAction.state == (int)State.hunt && creature.Hit(hitDamage))
            {
                // If target creature dies, gather food
                creature.GatherFood(inventory);

                // Then check if inventory is full, if so stores resources
                if (inventory.AtCapacity() && !FindStorage())
                {
                    //Idle for now, but needs to be able to go back to store state as soon as storage is built
                    Idle();
                }
            }
        }
        interactRoutine = null;
    }

    IEnumerator RepairRoutine()
    {
        yield return new WaitForSeconds(repairTime);
        if (currentAction.target.interact != null && (currentAction.target.interact as Building).Repair())
        {
            Idle();
        }
        interactRoutine = null;
    }

    public override void Save(AIData data)
    {
        base.Save(data);
        data.inventory = inventory;
    }

    public override void Load(AIData data)
    {
        base.Load(data);
        inventory = data.inventory;
    }
}
