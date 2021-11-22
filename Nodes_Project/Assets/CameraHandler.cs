using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private Camera cameraFollow;

    private Vector3 cameraFollowPosition;
    private bool edgeScrolling;
    private float cameraZoom = 8;
    private NodeView draggingNode = null;

    [Header("Variables de velocidad")]
    [SerializeField]private float cameraMoveSpeed = 3f;
    [SerializeField] private float cameraZoomSpeed = 10f;
    [SerializeField]private float moveAmount = 10f;
    [SerializeField]private float edgeSize = 30f;
    [SerializeField]private float zoomChangeAmount = 80f;

    [Header("Boundries")]
    [SerializeField] private Vector2 zoomBoundries = new Vector2(5, 20);
    [SerializeField] private Rect movementBoundries;

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
        
        ZoomMovementFunction();                   
    }    

    void LateUpdate()
    {
        HandleMovement();
        HandleZoom();
        MoveByDrag();
        MoveNodeByDrag();
    }

    void HandleMovement()
    {
        cameraFollowPosition.z = transform.position.z;
        Vector3 cameraMoveDir = (cameraFollowPosition - transform.position).normalized;
        float distance = Vector3.Distance(cameraFollowPosition, transform.position);

        if (distance > 0)
        {
            Vector3 newCameraPosition = transform.position + (cameraMoveDir * distance * cameraMoveSpeed * Time.unscaledDeltaTime);

            float x = Mathf.Clamp(newCameraPosition.x, movementBoundries.x - movementBoundries.width, movementBoundries.x + movementBoundries.width);
            float y = Mathf.Clamp(newCameraPosition.y, movementBoundries.y - movementBoundries.height, movementBoundries.y + movementBoundries.height);

            newCameraPosition = new Vector3(x, y, newCameraPosition.z);

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

        cameraFollow.orthographicSize += cameraZoomDifference * cameraZoomSpeed * Time.unscaledDeltaTime;

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
            cameraFollowPosition.y += moveAmount * Time.unscaledDeltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            cameraFollowPosition.y -= moveAmount * Time.unscaledDeltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            cameraFollowPosition.x -= moveAmount * Time.unscaledDeltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            cameraFollowPosition.x += moveAmount * Time.unscaledDeltaTime;
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
                cameraFollowPosition.x += moveAmount * Time.unscaledDeltaTime;
            }
            if (Input.mousePosition.x < edgeSize)
            {
                cameraFollowPosition.x -= moveAmount * Time.unscaledDeltaTime;
            }
            if (Input.mousePosition.y > Screen.height - edgeSize)
            {
                cameraFollowPosition.y += moveAmount * Time.unscaledDeltaTime;
            }
            if (Input.mousePosition.y < edgeSize)
            {
                cameraFollowPosition.y -= moveAmount * Time.unscaledDeltaTime;
            }
        }
    }

    private void ZoomMovementFunction()
    {
        if(Input.GetKey(KeyCode.KeypadPlus))
        {
            cameraZoom -= zoomChangeAmount * Time.unscaledDeltaTime;
        }

        if(Input.GetKey(KeyCode.KeypadMinus))
        {
            cameraZoom += zoomChangeAmount * Time.unscaledDeltaTime;
        }

        if(Input.mouseScrollDelta.y > 0)
        {
            cameraZoom -= zoomChangeAmount * Time.unscaledDeltaTime;
        }
        if(Input.mouseScrollDelta.y < 0)
        {
            cameraZoom += zoomChangeAmount * Time.unscaledDeltaTime;
        }
        cameraZoom = Mathf.Clamp(cameraZoom, zoomBoundries.x, zoomBoundries.y);
    }

    private void MoveByDrag()
    {
        if (draggingNode != null) return;
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

    void MoveNodeByDrag()
    {
        if (Input.GetMouseButton(0))
        {
            if (draggingNode == null)
            {
                RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward);
                foreach(var hit in hits)
                {
                    if(hit.collider != null)
                    {
                        try
                        {
                            draggingNode = hit.collider.GetComponentInChildren<NodeView>();
                        }
                        catch
                        {
                            draggingNode = null;
                        }
                    }
                    if (draggingNode != null)
                        break;
                }               
            }

            if (drag == false)
            {
                drag = true;
            }
        }
        else
        {
            drag = false;
            draggingNode = null;
        }

        if (drag && draggingNode != null)
        {
            draggingNode.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y, draggingNode.transform.position.z);
        }
    }

}
