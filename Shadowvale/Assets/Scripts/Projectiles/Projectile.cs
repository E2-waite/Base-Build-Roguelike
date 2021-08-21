using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum Type
    {
        arrow,
        shadow,
        necro
    }
    public Type type;
    public Interaction target, origin;
    public float speed;
    public int damage;
    public bool rotate = false;
    private bool move = false;
    private void Start()
    {
        Grid.projectiles.Add(this);
    }
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

                if (rotate)
                {
                    Vector3 dir = target.transform.position - transform.position;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.left);
                }
                float dist = Vector3.Distance(transform.position, target.transform.position);
                if (dist <= 0.1f)
                {
                    move = false;
                    Grid.projectiles.Remove(this);
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
