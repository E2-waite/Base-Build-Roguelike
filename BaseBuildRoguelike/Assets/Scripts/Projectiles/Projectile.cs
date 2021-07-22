using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    bool move = false;
    Interaction target, origin;
    float speed;
    int damage;
    private void Update()
    {
        if (move)
        {
            if (target == null)
            {
                Destroy(gameObject);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
                float dist = Vector3.Distance(transform.position, target.transform.position);
                if (dist <= 0.1f)
                {
                    move = false;
                    Hit(target, origin, damage);
                }
            }
        }
    }
    public void Setup(Interaction _to, Interaction _from, float _speed, int _damage = 0)
    {
        target = _to;
        origin = _from;
        speed = _speed;
        damage = _damage;
        move = true;
    }


    public virtual void Hit(Interaction to, Interaction from, int damage)
    {

    }
}
