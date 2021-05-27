using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    Inventory inventory;
    public GameObject marker;
    public Interaction target;
    public float speed = 15, gatherTime = 2;
    public bool canGather = true;

    public enum State
    {
        idle,
        move,
        gather,
        store,
        build
    }

    public State currentState = State.idle;

    private void Start()
    {
        inventory = GetComponent<Inventory>();
    }

    public void Direct(Vector2 pos, GameObject obj)
    {
        StopAllCoroutines();
        canGather = true;

        if (obj != null)
        {
            Debug.Log(obj.name);
            target = obj.GetComponent<Interaction>();

            if (target.type == Interaction.Type.resource)
            {
                if (inventory.AtCapacity())
                {
                    currentState = State.store;
                }
                else
                {
                    currentState = State.gather;
                }
            }
            else if (target.type == Interaction.Type.building)
            {
                if (target.building.type == Structure.Type.storage)
                {
                    currentState = State.store;
                }
            }
            marker.transform.position = obj.transform.position;
        }
        else
        {
            target = null;
            marker.transform.position = pos;
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
            }
        }
        else if (currentState == State.gather)
        {
            if (target == null)
            {
                currentState = State.idle;
            }
            else
            {
                if (Vector2.Distance(transform.position, target.transform.position) <= 0.1f)
                {
                    Debug.Log("In range");
                    if (canGather)
                    {
                        Debug.Log("Can gather");
                        StartCoroutine(GatherRoutine());
                    }
                }
                else
                {
                    Debug.Log("Moving towards resource");
                    transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
                }
            }
        }
        else if (currentState == State.store)
        {
            if (target == null)
            {
                currentState = State.idle;
            }
            else
            {
                if (Vector2.Distance(transform.position, target.transform.position) <= 0.1f)
                {
                    if (target.building.storage.type == Resource.Type.wood)
                    {
                        target.building.storage.Store(ref inventory.wood);
                    }
                }
                else
                {
                    Debug.Log("Moving towards storage");
                    transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
                }
            }
        }
    }

    IEnumerator GatherRoutine()
    {
        canGather = false;
        yield return new WaitForSeconds(gatherTime);

        if (target != null)
        {
            target.GatherResource(inventory);
        }

        if (inventory.AtCapacity())
        {
            currentState = State.store;
        }

        canGather = true;
    }

    //IEnumerator BuildRoutine()
    //{

    //}
}
