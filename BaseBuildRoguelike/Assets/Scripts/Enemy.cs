using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum Type
    {
        necromancer,
        undeadFollower,
        undeadSoldier
    }
    public Type type;

    public float speed;
    Animator anim;

    GameObject target;
    private void Start()
    {
        anim = GetComponent<Animator>();
        target = GameController.Instance.homeBuilding;
    }

    private void Update()
    {
        if (target == null)
        {
            target = GameController.Instance.homeBuilding;
        }
        else
        {
            float diff = target.transform.position.y - transform.position.y;
            anim.SetInteger("Direction", Mathf.RoundToInt(diff));
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }
    }
}
