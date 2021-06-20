using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseCollider : MonoBehaviour
{
    Necromancer necromancer;
    private void Start()
    {
        necromancer = transform.parent.GetComponent<Necromancer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 12)
        {
            necromancer.corpses.Add(collision.GetComponent<Corpse>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 12)
        {
            necromancer.corpses.Remove(collision.GetComponent<Corpse>());
        }
    }
}
