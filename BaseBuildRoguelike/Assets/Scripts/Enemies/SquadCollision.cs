using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadCollision : MonoBehaviour
{
    Enemy enemy;
    void Start()
    {
        enemy = transform.parent.GetComponent<Enemy>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            enemy.JoinSquad(collision.GetComponent<Enemy>());
        }
    }
}
