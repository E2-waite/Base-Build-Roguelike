using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    Inventory inventory;
    public GameObject marker;
    public Resource targetResource;
    public float speed = 15, gatherTime = 2;
    bool gathering = false;
    private void Start()
    {
        inventory = GetComponent<Inventory>();
    }

    public void Direct(Vector2 pos, GameObject obj)
    {
        StopAllCoroutines();
        gathering = false;
        if (obj != null)
        {
            targetResource = obj.GetComponent<Resource>();
            marker.transform.position = obj.transform.position;
        }
        else
        {
            targetResource = null;
            marker.transform.position = pos;
        }
    }

    private void Update()
    {
        if (transform.position != marker.transform.position)
        {
            transform.position = Vector2.MoveTowards(transform.position, marker.transform.position, speed * Time.deltaTime);
        }

        if (!gathering && targetResource != null && Vector2.Distance(transform.position, targetResource.transform.position) <= 0.1f)
        {
            StartCoroutine(GatherRoutine());
        }
    }

    IEnumerator GatherRoutine()
    {
        gathering = true;
        yield return new WaitForSeconds(gatherTime);
        targetResource.Gather(inventory);
        gathering = false;
    }
}
