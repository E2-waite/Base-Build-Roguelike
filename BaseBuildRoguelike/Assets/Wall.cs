using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    Grid grid;
    public Sprite[] wallSprites;

    private void Start()
    {
        grid = GameController.Instance.grid;
    } 
}
