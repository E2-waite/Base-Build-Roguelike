using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoSingleton<GameController>
{
    Grid grid;
    MouseControl mouse;
    FollowerController follower;
    public Vector2Int startPos;

    public enum Mode
    {
        build,
        direct,
        select
    }

    public Mode mode = Mode.select;

    void Start()
    {
        grid = GetComponent<Grid>();
        startPos = new Vector2Int((int)((grid.mapSize / 2) * grid.tileSize), (int)((grid.mapSize / 2) * grid.tileSize));
        grid.Generate(startPos);
        mouse = GetComponent<MouseControl>();
        mouse.camera.transform.position = new Vector3(startPos.x, startPos.y, mouse.camera.transform.position.z);
        follower = GetComponent<FollowerController>();
        follower.SpawnFollower(startPos);

    }


}
