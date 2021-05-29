using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    Animator anim;
    Inventory inventory;
    public GameObject marker;
    public Interaction target, lastTarget;
    public float speed = 15, gatherTime = 2, buildTime = 1;
    public bool canGather = true, canBuild = true, selected = false;
    public LayerMask resourceMask;
    public GameObject highlight;
    public enum State
    {
        idle,
        move,
        chopWood,
        mineStone,
        store,
        build
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

            if (target.type == Interaction.Type.resource)
            {
                if (inventory.AtCapacity())
                {
                    FindStorage();
                }
                else
                {
                    if (target.resource.type == Resource.Type.wood)
                    {
                        currentState = State.chopWood;
                    }
                    else if (target.resource.type == Resource.Type.stone)
                    {
                        currentState = State.mineStone;
                    }
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

            lastTarget = target;
            lastState = currentState;

            marker.transform.position = obj.transform.position;
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
                if ((currentState == State.chopWood || currentState == State.mineStone) && !inventory.AtCapacity())
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

        if (!FindStorage() && (lastState == State.chopWood || lastState == State.mineStone))
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
            Debug.Log("No Resource Storage");
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
        float closestDist = 10000;
        Building closestBuilding = null;
        List<Building> storage = new List<Building>();

        if (inventory.wood > 0)
        {
            storage.AddRange(BuildingController.Instance.woodPiles);
        }
        if (inventory.stone > 0)
        {
            storage.AddRange(BuildingController.Instance.stonePiles);
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
        // Find closest resource 
        List<Interaction> resources = new List<Interaction>();
        if (lastState == State.chopWood)
        {
            resources = GameController.Instance.grid.trees;
        }

        if (lastState == State.mineStone)
        {
            resources = GameController.Instance.grid.stones;
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
        canGather = false;
        yield return new WaitForSeconds(gatherTime);

        if (target != null)
        {
            target.resource.Gather(inventory);
        }

        if (inventory.AtCapacity())
        { 
            lastTarget = target;
            lastState = currentState;
            FindStorage();
        }

        canGather = true;
    }

    IEnumerator BuildRoutine()
    {
        canBuild = false;
        yield return new WaitForSeconds(buildTime);

        if (target != null)
        {
            target.building.construct.Build();
        }

        canBuild = true;
    }
}
