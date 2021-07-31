using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSortingLayer : MonoBehaviour
{
    public bool update = false;
    SpriteRenderer rend;
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        rend.sortingOrder = (int)-(transform.position.y * 10);
    }

    // Update is called once per frame
    void Update()
    {
        if (update)
        {
            rend.sortingOrder = (int)-(transform.position.y * 10);
        }
    }
}
