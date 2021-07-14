using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControl : MonoBehaviour
{
    Spawner spawner;
    public Camera camera;
    public float camSpeed = 50, camDist = 10, camMaxZoom = 25, camMinZoom = 10;
    public LayerMask tileMask, selectMask, directMask;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        spawner = Spawner.Instance;
    }

    private void Update()
    {
        ClickControl();
        CameraControl();
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
        Vector3 targetPos = new Vector3(GameController.Instance.startPos.x, GameController.Instance.startPos.y, camera.transform.position.z);
        while (camera.transform.position != targetPos)
        {
            camera.transform.position = Vector3.MoveTowards(camera.transform.position, targetPos, (camSpeed * 2) * Time.deltaTime);
            yield return null;
        }
        camRecentering = false;
    }

    void ClickControl()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        if (GameController.Instance.mode == GameController.Mode.build)
        {
            // Select tile below cursor
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, 0, tileMask);

            if (!Grid.IsSelected(hit.collider))
            {
                Grid.SelectTile(hit.collider, Spawner.Instance.selectedTemplate);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (GameController.Instance.mode == GameController.Mode.build)
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
                        GameController.Instance.mode = GameController.Mode.select;
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
                        GameController.Instance.mode = GameController.Mode.direct;
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
            if (GameController.Instance.mode == GameController.Mode.build)
            {
                GameController.Instance.mode = GameController.Mode.select;
                Grid.DeselectTile();
            }
            else if (GameController.Instance.mode == GameController.Mode.direct)
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
}
