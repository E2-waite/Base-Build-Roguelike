using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public void Setup(Interaction to, Interaction from, float speed, int damage = 0)
    {
        StartCoroutine(Move(to, from, speed, damage));
    }

    IEnumerator Move(Interaction to, Interaction from, float speed, int damage = 0)
    {
        float dist = 9999;
        while (dist > 0.1f)
        {
            if (to == null)
            {
                Destroy(gameObject);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, to.transform.position, speed * Time.deltaTime);
                dist = Vector3.Distance(transform.position, to.transform.position);
            }
            yield return null;
        }
        Hit(to, from, damage);
    }

    public virtual void Hit(Interaction to, Interaction from, int damage)
    {

    }
}
