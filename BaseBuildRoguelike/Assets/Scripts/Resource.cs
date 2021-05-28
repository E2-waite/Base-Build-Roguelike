using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    Animator anim;
    public int val = 10;
    public enum Type
    {
        wood,
        stone,
        food
    }
    public Type type;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public bool Gather(Inventory inv)
    {
        StartCoroutine(HitRoutine());
        if (type == Type.wood)
        {
            inv.wood++;
        }
        if (type == Type.stone)
        {
            inv.stone++;
        }
        if (type == Type.food)
        {
            inv.food++;
        }

        val--;
        if (val <= 0)
        {
            Destroy(gameObject);
        }
        return true;
    }

    IEnumerator HitRoutine()
    {
        anim.SetBool("Hit", true);
        yield return new WaitForSeconds(0.1f);
        anim.SetBool("Hit", false);
    }
}
