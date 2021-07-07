using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoSingleton<GameController>
{
    public int[] resources = new int[Consts.NUM_RESOURCES];
    public int[] maxResources = new int[Consts.NUM_RESOURCES];
    public Grid grid;
    MouseControl mouse;
    FollowerController follower;
    EnemyController enemies;
    public Vector2Int startPos;

    public Interaction homeBuilding;
    public Inspector inspector;

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
        startPos = new Vector2Int((int)(grid.mapSize / 2), (int)(grid.mapSize / 2));
        grid.Generate(startPos);
        mouse = GetComponent<MouseControl>();
        mouse.camera.transform.position = new Vector3(startPos.x, startPos.y, mouse.camera.transform.position.z);
        follower = GetComponent<FollowerController>();
        enemies = GetComponent<EnemyController>();
        follower.SpawnFollower(new Vector3(startPos.x, startPos.y, 0));
        enemies.StartSpawning();
        BuildingController.Instance.SpawnHome(grid.tiles[startPos.x, startPos.y]);
        homeBuilding = grid.tiles[startPos.x, startPos.y].structure;
    }

    public void AdjustResources(Resource.Type type, int val, int maxVal)
    {
        int pos = (int)type;
        resources[pos] += val;
        maxResources[pos] += maxVal;

        HUD.Instance.UpdateResources(resources, maxResources);
    }
}
