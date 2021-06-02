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
    public int maxHealth = 3, health, hitDamage = 1;
    public float speed;
    SpriteRenderer rend;
    Animator anim;

    GameObject target;
    private void Start()
    {
        health = maxHealth;
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
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

    public bool Hit(int damage, Follower attacker)
    {
        Debug.Log("HIT");
        health -= damage;
        target = attacker.gameObject;
        StartCoroutine(HitRoutine());

        if (health <= 0)
        {
            return true;
        }
        return false;
    }

    IEnumerator HitRoutine()
    {
        rend.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.color = Color.white;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
