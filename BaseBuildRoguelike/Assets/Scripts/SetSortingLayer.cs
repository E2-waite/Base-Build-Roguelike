using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSortingLayer : MonoBehaviour
{
    public bool update = false, tallTop = false;
    SpriteRenderer rend, parentRend;
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        if (!tallTop)
        {
            rend.sortingOrder = (int)-(transform.position.y * 10);
        }
        else
        {
            parentRend = transform.parent.GetComponent<SpriteRenderer>();
            rend.sortingOrder = parentRend.sortingOrder;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (update)
        {
            if (!tallTop)
            {
                rend.sortingOrder = (int)-(transform.position.y * 10);
            }
            else
            {
                rend.sortingOrder = parentRend.sortingOrder;
            }
        }
    }
}
