using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadCollision : MonoBehaviour
{
    Enemy enemy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enemy == null)
        {
            enemy = transform.parent.GetComponent<Enemy>();
        }

        if (collision.CompareTag("Enemy"))
        {
            enemy.JoinSquad(collision.GetComponent<Enemy>());
        }
    }
}
