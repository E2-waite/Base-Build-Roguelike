using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    Animator anim;
    Inventory inventory;
    public GameObject marker;
    public Interaction target, lastTarget;
    public float speed = 15, gatherTime = 2, buildTime = 1, hitTime = 0.5f;
    public int hitDamage = 1;
    public bool canGather = true, canBuild = true, canHit = true, selected = false;
    public LayerMask resourceMask;
    public GameObject highlight;
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

    public State currentState = State.idle, lastState = State.idle;

    private void Start()
    {
        inventory = GetComponent<Inventory>();
        anim = GetComponent<Animator>();
    }

    public void Select()
    {
        highlight.SetActive(true);
        selected = true;
    }

    public void Deselect()
    {
        highlight.SetActive(false);
        selected = false;
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
            Debug.Log(obj.name);
            target = obj.GetComponent<Interaction>();

            marker.transform.position = obj.transform.position;


            if (target.type == Interaction.Type.resource)
            {
                if (target.resource.type == Resource.Type.wood)
                {
                    currentState = State.chopWood;
                }
                else if (target.resource.type == Resource.Type.stone)
                {
                    currentState = State.mineStone;
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
                        currentState = State.store;
                    }
                }
                else
                {
                    currentState = State.build;
                }
            }
            else if (target.type == Interaction.Type.creature)
            {
                currentState = State.hunt;

                if (inventory.AtCapacity())
                {
                    FindStorage();
                }
            }

            lastTarget = target;
            lastState = currentState;
        }
        else
        {
            target = null;
            currentState = State.move;
        }
    }

    private void Update()
    {
        if (currentState == State.move)
        {
            if (transform.position == marker.transform.position)
            {
                currentState = State.idle;
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, marker.transform.position, speed * Time.deltaTime);
                float diff =  marker.transform.position.y - transform.position.y;
                anim.SetInteger("Direction", Mathf.RoundToInt(diff));
            }
        }
        else
        {
            if (target == null)
            {
                if ((currentState == State.chopWood || currentState == State.mineStone || currentState == State.hunt) && !inventory.AtCapacity())
                {
                    target = FindResource();
                }
                else
                {
                    currentState = State.idle;
                }
            }
            else
            {
                float diff = target.transform.position.y - transform.position.y;
                anim.SetInteger("Direction", Mathf.RoundToInt(diff));
                if (Vector2.Distance(transform.position, target.transform.position) <= 0.25f)
                {
                    if ((currentState == State.chopWood || currentState == State.mineStone) && canGather)
                    {
                        StartCoroutine(GatherRoutine());
                    }
                    else if (currentState == State.store)
                    {
                        Store();
                    }
                    else if (currentState == State.build && canBuild)
                    {
                        StartCoroutine(BuildRoutine());
                    }
                    else if (currentState == State.hunt && canHit)
                    {
                        StartCoroutine(HitRoutine());
                    }
                }
                else
                {
                    transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
                }
            }
        }
    }

    void Store()
    {
        if (target.building.storage.type == Resource.Type.wood)
        {
            target.building.storage.Store(ref inventory.wood);
        }
        else if (target.building.storage.type == Resource.Type.stone)
        {
            target.building.storage.Store(ref inventory.stone);
        }
        else if (target.building.storage.type == Resource.Type.food)
        {
            target.building.storage.Store(ref inventory.food);
        }

        if (!FindStorage() && (lastState == State.chopWood || lastState == State.mineStone || lastState == State.hunt))
        {
            currentState = lastState;
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
        Debug.Log("Finding Storage");
        target = ClosestStorage();
        if (target == null)
        {
            Debug.Log("Either no resource storage available, or nothing to store...");
            return false;
        }
        else
        {
            currentState = State.store; 
            return true;
        }
    }

    Interaction ClosestStorage()
    {
        // Returns closest resource storage
        float closestDist = 10000;
        Building closestBuilding = null;
        List<Building> storage = new List<Building>();

        // Only checks resources that are in the follower's inventory
        if (inventory.wood > 0)
        {
            storage.AddRange(BuildingController.Instance.woodPiles);
        }
        if (inventory.stone > 0)
        {
            storage.AddRange(BuildingController.Instance.stonePiles);
        }
        if (inventory.food > 0)
        {
            storage.AddRange(BuildingController.Instance.foodPiles);
        }

        foreach (Building building in storage)
        {
            float dist = Vector2.Distance(transform.position, building.transform.position);
            if (dist < closestDist && building.storage.currentStorage < building.storage.maxStorage)
            {
                closestDist = dist;
                closestBuilding = building;
            }
        }

        Interaction interaction = null;

        if (closestBuilding != null)
        {
            interaction = closestBuilding.interaction;
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

        currentState = lastState;

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
            if (currentState == State.hunt && target.creature.Hit(hitDamage))
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
