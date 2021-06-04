using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    public enum Type
    {
        worker,
        soldier,
        archer,
        priest
    }

    [Header("Follower Settings")]
    public Type type;
    public Squad squad;
    public int maxHealth = 10, health, hitDamage = 1;
    public float targetDist = 0.25f, speed = 5f;
    [HideInInspector] public Interaction interaction;
    Animator anim;
    SpriteRenderer rend;
    public GameObject highlight, marker;
    bool selected;


    private void Start()
    {
        health = maxHealth;
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        interaction = GetComponent<Interaction>();
        if (type == Type.worker)
        {
            Worker worker = GetComponent<Worker>();
            worker.Setup();
        }

    }

    public void Select()
    {
        highlight.SetActive(true);
        selected = true;
    }

    public void Deselect()
    {
        highlight.SetActive(false);
        selected = false;
    }

    public void Move(Vector3 position)
    {
        transform.position = Vector2.MoveTowards(transform.position, position, speed * Time.deltaTime);
        float diff = position.y - transform.position.y;
        anim.SetInteger("Direction", Mathf.RoundToInt(diff));
        if (squad != null)
        {
            foreach (Follower follower in squad.followers)
            {
                if (follower != this && follower != null)
                {
                    float dist = Vector3.Distance(transform.position, follower.transform.position);
                    if (dist <= 0.5)
                    {
                        Vector3 diffVec = transform.position - follower.transform.position;
                        transform.position = Vector2.MoveTowards(transform.position, transform.position + diffVec, speed * Time.deltaTime);
                    }
                }
            }
        }
    }

    public bool Hit(int damage, Enemy attacker)
    {
        health -= damage;
        StartCoroutine(HitRoutine());

        if (type == Type.soldier)
        {
            Soldier soldier = (Soldier)this;
            soldier.TargetEnemy(attacker);
        }

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
