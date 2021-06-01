using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoSingleton<GameController>
{
    public int wood = 0, stone = 0, food = 0, maxWood, maxStone, maxFood;
    public Grid grid;
    MouseControl mouse;
    FollowerController follower;
    public Vector2Int startPos;
    public GameObject firepitPrefab;

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
        follower.SpawnFollower(startPos);
        grid.tiles[startPos.x, startPos.y].structure = Instantiate(firepitPrefab, new Vector3(startPos.x, startPos.y, 0), Quaternion.identity);
    }

    public void AdjustResources(Resource.Type type, int val, int maxVal)
    {
        if (type == Resource.Type.wood)
        {
            wood += val;
            maxWood += maxVal;
        }
        else if (type == Resource.Type.stone)
        {
            stone += val;
            maxStone += maxVal;
        }
        else if (type == Resource.Type.food)
        {
            food += val;
            maxFood += maxVal;
        }

        HUD.Instance.UpdateResources(wood, maxWood, stone, maxStone, food, maxFood);
    }
}
