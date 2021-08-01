using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardDetect : MonoBehaviour
{
    GuardTower guardTower;
    private void Start()
    {
        guardTower = transform.parent.GetComponent<GuardTower>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("TRIGGER: " + collision.name);
        if (collision.CompareTag("Enemy"))
        {
            guardTower.inRange.Add(collision.gameObject.GetComponent<Enemy>());
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        guardTower.inRange.Remove(collision.GetComponent<Enemy>());
    }
}
