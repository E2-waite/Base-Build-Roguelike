using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSortingLayer : MonoBehaviour
{
    public bool update = false;
    SpriteRenderer rend;
    public SpriteRenderer underRend, overRend;
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        rend.sortingOrder = (int)-(transform.position.y * 10);
        if (underRend != null)
        {
            underRend.sortingOrder = rend.sortingOrder - 1;
        }
        if (overRend != null)
        {
            underRend.sortingOrder = rend.sortingOrder + 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (update)
        {
            rend.sortingOrder = (int)-(transform.position.y * 10);
            if (underRend != null)
            {
                underRend.sortingOrder = rend.sortingOrder - 1;
            }
            if (overRend != null)
            {
                underRend.sortingOrder = rend.sortingOrder + 1;
            }
        }
    }
}
