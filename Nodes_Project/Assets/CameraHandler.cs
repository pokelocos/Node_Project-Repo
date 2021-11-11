using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private Camera cameraFollow;

    private Vector3 cameraFollowPosition;
    private bool edgeScrolling;
    private float cameraZoom;

    [Header("Variables de velocidad")]
    [SerializeField]private float cameraMoveSpeed = 3f;
    [SerializeField] private float cameraZoomSpeed = 10f;
    [SerializeField]private float moveAmount = 10f;
    [SerializeField]private float edgeSize = 30f;
    [SerializeField]private float zoomChangeAmount = 80f;

    [Header("Boundries")]
    [SerializeField] Vector2 zoomBoundries = new Vector2(5, 20);

    bool drag = false;
    Vector3 difference;
    Vector3 origen;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {       
        HandleManualMovement();
        HandleEdgeScrolling();
        HandleMovement();

        ZoomMovementFunction();
        HandleZoom();             
    }    

    void LateUpdate()
    {
        MoveByDrag();
    }

    void HandleMovement()
    {
        cameraFollowPosition.z = transform.position.z;
        Vector3 cameraMoveDir = (cameraFollowPosition - transform.position).normalized;
        float distance = Vector3.Distance(cameraFollowPosition, transform.position);

        if (distance > 0)
        {
            Vector3 newCameraPosition = transform.position + (cameraMoveDir * distance * cameraMoveSpeed * Time.deltaTime);
            float distanceAfterMoving = Vector3.Distance(newCameraPosition, cameraFollowPosition);

            if (distanceAfterMoving > distance)
            {
                // Overshot the target
                newCameraPosition = cameraFollowPosition;
            }

            transform.position = newCameraPosition;
        }
    }

    void HandleZoom()
    {
        float cameraZoomDifference = cameraZoom - cameraFollow.orthographicSize;       

        cameraFollow.orthographicSize += cameraZoomDifference * cameraZoomSpeed * Time.deltaTime;

        if (cameraZoomDifference > 0)
        {
            if (cameraFollow.orthographicSize > cameraZoom)
            {
                cameraFollow.orthographicSize = cameraZoom;
            }
        }
        else
        {
            if (cameraFollow.orthographicSize < cameraZoom)
            {
                cameraFollow.orthographicSize = cameraZoom;
            }
        }
    }

    private void HandleManualMovement()
    {       
        if (Input.GetKey(KeyCode.W))
        {
            cameraFollowPosition.y += moveAmount * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            cameraFollowPosition.y -= moveAmount * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            cameraFollowPosition.x -= moveAmount * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            cameraFollowPosition.x += moveAmount * Time.deltaTime;
        }        
    }

    private void HandleEdgeScrolling()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            edgeScrolling = !edgeScrolling;
        }

        if(edgeScrolling)
        {           
            if(Input.mousePosition.x > Screen.width - edgeSize)
            {
                cameraFollowPosition.x += moveAmount * Time.deltaTime;
            }
            if (Input.mousePosition.x < edgeSize)
            {
                cameraFollowPosition.x -= moveAmount * Time.deltaTime;
            }
            if (Input.mousePosition.y > Screen.height - edgeSize)
            {
                cameraFollowPosition.y += moveAmount * Time.deltaTime;
            }
            if (Input.mousePosition.y < edgeSize)
            {
                cameraFollowPosition.y -= moveAmount * Time.deltaTime;
            }
        }
    }

    private void ZoomMovementFunction()
    {
        if(Input.GetKey(KeyCode.KeypadPlus))
        {
            cameraZoom -= zoomChangeAmount * Time.deltaTime;
        }

        if(Input.GetKey(KeyCode.KeypadMinus))
        {
            cameraZoom += zoomChangeAmount * Time.deltaTime;
        }

        if(Input.mouseScrollDelta.y > 0)
        {
            cameraZoom -= zoomChangeAmount * Time.deltaTime;
        }
        if(Input.mouseScrollDelta.y < 0)
        {
            cameraZoom += zoomChangeAmount * Time.deltaTime;
        }
        cameraZoom = Mathf.Clamp(cameraZoom, zoomBoundries.x, zoomBoundries.y);
    }

    private void MoveByDrag()
    {
        if (Input.GetMouseButton(0))
        {
            difference = (cameraFollow.ScreenToWorldPoint(Input.mousePosition)) - cameraFollow.transform.position;
            if (drag == false)
            {
                drag = true;
                origen = cameraFollow.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            drag = false;
        }

        if (drag)
        {           
            transform.position = (origen - difference);
            cameraFollowPosition = transform.position;
        }
    }
}
