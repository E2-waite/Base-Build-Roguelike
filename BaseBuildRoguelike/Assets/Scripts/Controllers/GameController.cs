using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoSingleton<GameController>
{
    public enum GameState
    { 
        select,
        build,
        direct
    }
    public bool loadGame = true;
    public GameState gameState;
    private GridBuilder grid;
    private Spawner spawner;
    private Save save;
    private Load load;
    public Inspector inspector;
    public Camera camera;
    public float camSpeed = 50, camDist = 10, camMaxZoom = 20, camMinZoom = 5;
    public LayerMask tileMask, selectMask, directMask;

    void Start()
    {
        load = GetComponent<Load>();
        spawner = Spawner.Instance;
        if (!loadGame || !load.LoadGame())
        {
            GridBuilder.Instance.Generate();
            spawner.Setup();
            spawner.SpawnFollower(new Vector3(Grid.startPos.x, Grid.startPos.y, 0));
            spawner.SpawnHome(Grid.tiles[Grid.startPos.x, Grid.startPos.y]);
        }

        Grid.startPos = new Vector2Int((int)(Grid.size / 2), (int)(Grid.size / 2));




        Cursor.lockState = CursorLockMode.Confined;
        camera.transform.position = new Vector3(Grid.startPos.x, Grid.startPos.y, camera.transform.position.z);

        save = GetComponent<Save>();
        //save.SaveGame();
    }

    private void Update()
    {
        ClickControl();
        CameraControl();

        if (Input.GetKey(KeyCode.Escape))
        {
            save.SaveGame();
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            save.SaveGame();
        }
    }


    void ClickControl()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        if (gameState == GameState.build)
        {
            // Select tile below cursor
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, 0, tileMask);

            if (!Grid.IsSelected(hit.collider))
            {
                Spawner spawner = Spawner.Instance;
                Grid.SelectTile(hit.collider, spawner.buildings[spawner.selectedTemplate]);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (gameState == GameState.build)
            {
                // Place building on selected tile
                spawner.BuildStructure(Grid.selected);
            }
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, 0, selectMask);
                if (hit.collider == null)
                {
                    if (!GameController.Instance.inspector.mouseOver)
                    {
                        Followers.Deselect();
                        Buildings.Deselect();
                        gameState = GameState.select;
                    }
                }
                else
                {
                    Interaction target = hit.collider.GetComponent<Interaction>();
                    // Select either the follower or building in clicked position
                    if (target is Follower)
                    {
                        Followers.Select(target as Follower);
                        Buildings.Deselect();
                        gameState = GameState.direct;
                        return;
                    }
                    else if (target is Building)
                    {
                        Followers.Deselect();
                        Buildings.Select(hit.collider.gameObject);
                        return;
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (gameState == GameState.build)
            {
                gameState = GameState.select;
                Grid.DeselectTile();
            }
            else if (gameState == GameState.direct)
            {
                // Direct target follower
                GameObject targetObj = null;
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, 0, directMask);

                if (hit.collider != null)
                {
                    targetObj = hit.collider.gameObject;
                }

                Followers.Direct(mousePos2D, targetObj);
            }
        }
    }

    void CameraControl()
    {
        Vector2 mousePos = Input.mousePosition;

        if (Input.GetKeyDown(KeyCode.H) && !camRecentering)
        {
            StartCoroutine(RecenterCam());
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f && camera.orthographicSize > camMinZoom) // forward
        {
            camera.orthographicSize--;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && camera.orthographicSize < camMaxZoom) // backwards
        {
            camera.orthographicSize++;
        }

        if ((mousePos.x <= 10 && Input.GetAxis("Mouse X") < 0) || (mousePos.x >= Screen.width - 10 && Input.GetAxis("Mouse X") > 0))
        {
            Vector3 newPos = new Vector3(camera.transform.position.x + (Input.GetAxis("Mouse X") * (camSpeed * Time.deltaTime)), camera.transform.position.y, -camDist);
            camera.transform.position = newPos;
        }

        if ((mousePos.y <= 10 && Input.GetAxis("Mouse Y") < 0) || (mousePos.y >= Screen.height - 10 && Input.GetAxis("Mouse Y") > 0))
        {
            Vector3 newPos = new Vector3(camera.transform.position.x, camera.transform.position.y + (Input.GetAxis("Mouse Y") * (camSpeed * Time.deltaTime)), -camDist);
            camera.transform.position = newPos;
        }
    }

    bool camRecentering = false;
    IEnumerator RecenterCam()
    {
        camRecentering = true;
        Vector3 targetPos = new Vector3(Grid.startPos.x, Grid.startPos.y, camera.transform.position.z);
        while (camera.transform.position != targetPos)
        {
            camera.transform.position = Vector3.MoveTowards(camera.transform.position, targetPos, (camSpeed * 2) * Time.deltaTime);
            yield return null;
        }
        camRecentering = false;
    }

    private void OnApplicationQuit()
    {
        //save.SaveGame();
    }
}
