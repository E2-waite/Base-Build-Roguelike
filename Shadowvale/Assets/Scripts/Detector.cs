using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    public Follower follower;
    public List<Interaction> detected = new List<Interaction>();
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("DETECT");
        Interaction target = collision.GetComponent<Interaction>();
        if (follower.currentAction.state == 0)
        {
            Debug.Log("SHOULD UPDATE");
            follower.UpdateTarget(target);
        }
        else
        {
            detected.Add(target);
        }
    }

    public bool GetTarget(ref Interaction target)
    {
        if (detected.Count > 0)
        {
            target = detected[0];
            detected.RemoveAt(0);
        }
        return false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Interaction target = collision.GetComponent<Interaction>();
        detected.Remove(target);
    }
}
