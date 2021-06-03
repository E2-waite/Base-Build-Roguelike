using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum Type
    {
        necromancer,
        undead
    }
    [Header("Enemy Settings")]
    public Type type;
    public int maxHealth = 3, health, hitDamage = 1;
    public float speed = 2, targetDist = 0.25f;
    [HideInInspector] public Interaction interaction;
    SpriteRenderer rend;
    Animator anim;
    public Interaction target;
    public LayerMask buildingMask;
    private void Start()
    {
        health = maxHealth;
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        interaction = GetComponent<Interaction>();
        target = GameController.Instance.homeBuilding;
    }
    public void Move(Vector3 position)
    {
        transform.position = Vector2.MoveTowards(transform.position, position, speed * Time.deltaTime);
        float yDiff = position.y - transform.position.y, xDiff = position.x - transform.position.x;
        anim.SetInteger("Direction", Mathf.RoundToInt(yDiff));
        RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(xDiff, yDiff), .2f, buildingMask);

        if (hit.collider != null && target.type != Interaction.InteractionType.follower && target.gameObject != hit.collider.gameObject)
        {
            target = hit.collider.GetComponent<Interaction>();
        }
    }

    // GET HIT
    public bool Hit(int damage, Follower attacker)
    {
        Debug.Log("HIT");
        health -= damage;
        target = attacker.interaction;
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
