using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControl : MonoBehaviour
{
    public GameObject camera;
    public float camSpeed = 50, camDist = 10;
    FollowerController followers;
    Grid grid;
    public LayerMask clickMask, taskMask;
    private void Start()
    {
        followers = GetComponent<FollowerController>();
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
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, clickMask);

            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Follower"))
                {
                    followers.SelectFollower(hit.collider);
                    grid.DeselectTile();
                    return;
                }
                else if (hit.collider.CompareTag("Tile"))
                {
                    grid.SelectTile(hit.collider);
                    followers.DeselectFollower();
                    return;
                }
                else
                {
                    grid.DeselectTile();
                    followers.DeselectFollower();
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);


            GameObject targetObj = null;

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, taskMask);

            if (hit.collider != null && hit.collider.CompareTag("Resource"))
            {
                targetObj = hit.collider.gameObject;
            }

            followers.DirectFollower(mousePos2D, targetObj);
        }
    }
}
