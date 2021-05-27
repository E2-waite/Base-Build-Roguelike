using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControl : MonoBehaviour
{
    public GameObject camera;
    public float camSpeed = 50, camDist = 10;
    FollowerController followers;
    BuildingController buildings;
    Grid grid;
    public LayerMask tileMask, selectMask, directMask;
    private void Start()
    {
        followers = GetComponent<FollowerController>();
        buildings = GetComponent<BuildingController>();
        grid = GetComponent<Grid>();
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        ClickControl();
        CameraControl();
    }

    void CameraControl()
    {
        Vector2 mousePos = Input.mousePosition;

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

    void ClickControl()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        if (GameController.Instance.mode == GameController.Mode.build)
        {
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, 0, tileMask);

            if (!grid.IsSelected(hit.collider))
            {
                grid.SelectTile(hit.collider);
            }
            // Build on tile
        }

        if (Input.GetMouseButtonDown(0))
        {



            if (GameController.Instance.mode == GameController.Mode.build)
            {
                // Build on selected tile
                buildings.Build(grid.selected);
            }
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, 0, selectMask);
                if (hit.collider == null)
                {
                    followers.DeselectFollower();
                    GameController.Instance.mode = GameController.Mode.select;
                }
                else
                {
                    if (hit.collider.CompareTag("Follower"))
                    {
                        followers.SelectFollower(hit.collider);
                        GameController.Instance.mode = GameController.Mode.direct;
                        return;
                    }
                    else if (hit.collider.CompareTag("Building"))
                    {
                        followers.DeselectFollower();
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
                grid.DeselectTile();
            }
            else if (GameController.Instance.mode == GameController.Mode.direct)
            { 
                GameObject targetObj = null;
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, 0, directMask);

                if (hit.collider != null)
                {
                    targetObj = hit.collider.gameObject;
                }

                followers.DirectFollower(mousePos2D, targetObj);
            }
        }
    }
}
