using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoSingleton<GameController>
{
    Grid grid;
    MouseControl mouse;
    FollowerController follower;
    public Vector2Int startPos;


    void Start()
    {
        grid = GetComponent<Grid>();
        startPos = new Vector2Int((int)Random.Range(0, grid.mapSize * grid.tileSize), (int)Random.Range(0, grid.mapSize * grid.tileSize));
        grid.Generate(startPos);
        mouse = GetComponent<MouseControl>();
        mouse.camera.transform.position = new Vector3(startPos.x, startPos.y, mouse.camera.transform.position.z);
        follower = GetComponent<FollowerController>();
        follower.SpawnFollower(startPos);

    }


}
