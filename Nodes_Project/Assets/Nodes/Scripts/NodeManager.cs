using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RA.InputManager;

public class NodeManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Filters filter = Filters.NONE;

    //Visible variables
    [Header("Components")]
    [SerializeField] private ConnectionView proxyConnection;

    //Hidden variables
    private InputManager inputManager;
    private NodeController dragOriginNode;
    private static NodeManager _singleton;

    private NodeController targetNode;

    //All below this is deprecated;
    private NodeView overNode;  
    private NodeController destNode;
    private bool drag = false;
    [Header("References")]
    [SerializeField]
    private SpriteRenderer mouseIcon;
    [SerializeField]
    private TextMesh price_handler;
    [SerializeField]
    private TextMesh price_handler_shadow;
    [SerializeField]
    private SpriteRenderer mouseIcon_background;
    [SerializeField]
    private SpriteRenderer mouseIcon_ring;
    [SerializeField]
    private Sprite invalidIngredient;

    private float lastClickTime;
    private const float DOUBLE_CLICK_TIME = .2f;

    [SerializeField]
    private NodeConnectionDisplay connectionDisplay;
    [SerializeField]
    private Vector3 ConnectionsDisplayOffset = new Vector3(0, -50, 0);

    public AudioSource source;
    public AudioClip cannotConect;
    public AudioClip canConect;
    public AudioClip nothingConect;

    //Properties
    public static Filters Filter
    {
        get
        {
            if (_singleton == null)
                _singleton = FindObjectOfType<NodeManager>();

            return _singleton.filter;
        }
    }

    public enum Filters
    {
        NONE, CONNECTION_MODE
    }

    private void Start()
    {
        inputManager = FindObjectOfType<InputManager>();   
    }

    void Update()
    {
        ManageConnections();

        //Execute at the end.
        ManageColors();

        return;

        //All below this are deprecated.

        RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.forward);

        NodeView hitNode = null;
        NodeController nodeController = null;
        ConnectionView hitConnection = null;
        ConnectionController connectionController = null;

        mouseIcon.color = Color.clear;
        mouseIcon_background.color = Color.clear;
        mouseIcon_ring.color = Color.clear;

        Vector3 destPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0.7f, 0.7f, 0); 

        destPos.z = 0;

        mouseIcon.transform.position = destPos;

        price_handler.text = string.Empty;
        price_handler.GetComponent<MeshRenderer>().sortingLayerName = "Node";
        price_handler.GetComponent<MeshRenderer>().sortingOrder = 200;

        price_handler_shadow.text = string.Empty;
        price_handler_shadow.GetComponent<MeshRenderer>().sortingLayerName = "Node";
        price_handler_shadow.GetComponent<MeshRenderer>().sortingOrder = 200;

        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                if(hitNode == null)
                {
                    try
                    {
                        hitNode = hit.collider.GetComponentInChildren<NodeView>();
                        nodeController = hit.collider.GetComponentInChildren<NodeController>();
                    }
                    catch
                    {
                        hitNode = null;
                        nodeController = null;
                    }
                }

                if (hitConnection == null)
                {
                    try
                    {
                        hitConnection = hit.collider.GetComponentInChildren<ConnectionView>();
                        connectionController = hit.collider.GetComponentInChildren<ConnectionController>();
                    }
                    catch
                    {
                        hitConnection = null;
                        connectionController = null;
                    }
                }
            }
        }

        overNode = hitNode;

        if(overNode != null && !EventSystem.current.IsPointerOverGameObject())
        {      
            Vector3 screenPos = Camera.main.WorldToScreenPoint(overNode.transform.position);
            connectionDisplay.gameObject.SetActive(true);
            connectionDisplay.gameObject.transform.position = screenPos + ConnectionsDisplayOffset;

            connectionDisplay.EnableIOObjetcs(overNode.GetRecipes());
            /*Inputs*/connectionDisplay.SetInputsText(overNode.GetConnectedInputs() + "/" + overNode.GetInputs().Length);
            /*Outputs*/connectionDisplay.SetOutputsText(overNode.GetConnectedOutputs() + "/" + overNode.GetOutputs().Length);
        }
        else
        {
            connectionDisplay.gameObject.SetActive(false);
            
        }

        if (hitConnection && overNode == null)
        {
            if (hitConnection.GetIngredient() != null)
            {
                mouseIcon.sprite = hitConnection.GetIngredient().icon;
                mouseIcon.color = hitConnection.GetIngredient().color;

                Color darker = Color.Lerp(hitConnection.GetIngredient().color, Color.black, 0.5f);

                mouseIcon_background.color = darker;
                mouseIcon_ring.color = new Color(0, 0, 0, 0.15f);

                if (!Input.GetMouseButton(1))
                {
                    mouseIcon.transform.position = hitConnection.GetMiddlePoint();

                    if (hitConnection.GetIngredient() != null)
                    {
                        price_handler.text = "$" + hitConnection.GetIngredient().price;
                        price_handler_shadow.text = "$" + hitConnection.GetIngredient().price;
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            dragOriginNode = nodeController;

            float timeSinceLastClick = Time.unscaledTime - lastClickTime;

            if (timeSinceLastClick <= DOUBLE_CLICK_TIME)
            {
                if (hitConnection!= null)
                {
                    //hitConnection.Disconnect();
                    connectionController.Disconnect();
                }
            }

            lastClickTime = Time.unscaledTime;
        }

        if (Input.GetMouseButtonUp(1))
        {
            if (dragOriginNode != null && overNode != null && dragOriginNode != overNode)
            {
                destNode = nodeController;

                ConnectNodes(dragOriginNode, destNode);

                //if (originNode.CanConnectWith(destNode) == 2)
                //{
                //    originNode.ConnectWith(destNode);
                //    source.PlayOneShot(canConect);
                //}
                //else
                //{
                //    source.PlayOneShot(cannotConect);
                //}
            }
            else
            {
                source.PlayOneShot(nothingConect);
            }

            var connections = FindObjectsOfType<ConnectionView>().Where(x => x != proxyConnection).ToArray(); ;

            foreach (var connection in connections)
            {
                if (connection.GetIngredient() != null)
                {
                    connection.SetDotColor(connection.GetIngredient().color);
                }

                connection.Paint(connection.body_color, connection.border_color);
            }

            foreach (var node in FindObjectsOfType<NodeView>().Where(x => x != dragOriginNode).ToArray())
            {
                node.Paint(node.GetColor());
                node.SetBright(Color.clear);
            }

            dragOriginNode = null;
            destNode = null;
        }

        if (dragOriginNode != null && destNode == null)
        {
            Vector3 originPos = dragOriginNode.transform.position;
            destPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //if (originNode.GetCurrentRecipe() != null)
            //{
            //    for (int i = 0; i < originNode.GetOutputs().Length; i++)
            //    {
            //        if (originNode.GetOutputs()[i] == null)
            //        {
            //            var ingredient = originNode.GetCurrentRecipe().GetOutputs()[i];

            //            mouseIcon.sprite = ingredient.icon;
            //            mouseIcon.color = ingredient.color;

            //            Color darker = Color.Lerp(ingredient.color, Color.black, 0.5f);

            //            mouseIcon_background.color = darker;
            //            mouseIcon_ring.color = new Color(0, 0, 0, 0.15f);

            //            price_handler.text = "$" + ingredient.price;
            //            price_handler_shadow.text = "$" + ingredient.price;

            //            break;
            //        }
            //    }
            //}
            //else
            //{
            //    mouseIcon.sprite = invalidIngredient;
            //    mouseIcon.color = Color.red;

            //    Color darker = Color.Lerp(Color.red, Color.black, 0.5f);

            //    mouseIcon_background.color = darker;
            //    mouseIcon_ring.color = new Color(0, 0, 0, 0.15f);
            //}

            destPos.z = originPos.z;

            proxyConnection.gameObject.SetActive(true);

            proxyConnection.FollowPositions(originPos, destPos);

            var connections = FindObjectsOfType<ConnectionView>().Where(x => x != proxyConnection).ToArray();

            foreach (var connection in connections)
            {
                connection.SetDotColor(Color.grey);

                Color lightGray = Color.Lerp(Color.gray, Color.white, 0.5f);

                connection.Paint(lightGray, Color.gray);
            }

            //var nodes = FindObjectsOfType<NodeView>().Where(x => x != originNode).ToArray();

            //if (hitNode)
            //{
            //    if (hitNode != originNode)
            //    {
            //        int affinity = originNode.CanConnectWith(hitNode);

            //        switch (affinity)
            //        {
            //            case 0:
            //                Color lightGray = Color.Lerp(Color.gray, Color.white, 0.5f);
            //                auxiliarConection.SetLineColor(lightGray, Color.gray);
            //                break;

            //            case 1:
            //                Color lightOrange = Color.Lerp(new Color(1, 0.6f, 0, 1), Color.white, 0.5f);
            //                auxiliarConection.SetLineColor(lightOrange, new Color(1, 0.6f, 0, 1));
            //                break;

            //            case 2:
            //                Color lightGreen = Color.Lerp(new Color(0.7f, 0.9f, 0.3f), Color.white, 0.5f);
            //                auxiliarConection.SetLineColor(lightGreen, new Color(0.7f, 0.9f, 0.3f));
            //                break;
            //        }
            //    }
            //}
            //else
            //{
            //    Color lightGray = Color.Lerp(Color.gray, Color.white, 0.5f);
            //    auxiliarConection.SetLineColor(lightGray, Color.gray);
            //}

            //foreach (var node in nodes)
            //{
            //    int affinity = originNode.CanConnectWith(node);

            //    switch (affinity)
            //    {
            //        case 0:
            //            node.Paint(Color.gray);
            //            break;

            //        case 1:
            //            node.SetBright(new Color(1, 0.6f, 0, 1));
            //            break;

            //        case 2:
            //            node.SetBright(new Color(0.7f, 0.9f, 0.3f));
            //            break;
            //    }
            //}
            
        }
        else
        {
            proxyConnection.gameObject.SetActive(false);
        }

    }    

    /// <summary>
    /// Controls the logic of the connections.
    /// </summary>
    private void ManageConnections()
    {
        if (inputManager.OverObject is NodeController)
        {
            targetNode = inputManager.OverObject as NodeController;
        }
        else
        {
            targetNode = null;
        }

        // Proxy connection
        if (dragOriginNode != null && dragOriginNode.GetFreeOutput() != null)
        {
            Vector3 from = dragOriginNode.transform.position;
            Vector3 to = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            to.z = 0;

            proxyConnection.gameObject.SetActive(true);
            proxyConnection.FollowPositions(from, to);
        }
        else
        {
            proxyConnection.gameObject.SetActive(false);
        }

        // Input events

        if (Input.GetMouseButtonDown(1))
        {
            if (inputManager.OverObject is NodeController)
            {
                dragOriginNode = inputManager.OverObject as NodeController;
            }
        }

        if (Input.GetMouseButtonUp(1) && dragOriginNode)
        {
            if (targetNode && dragOriginNode.GetFreeOutput() != null && dragOriginNode.CanConnectWith(targetNode, dragOriginNode.GetFreeOutput().Product) == 1)
            {
                ConnectNodes(dragOriginNode, targetNode);
            }

            dragOriginNode = null;
        }

        if (inputManager.RightDoubleClick)
        {
            if (inputManager.OverObject is ConnectionController)
            {
                (inputManager.OverObject as ConnectionController).Disconnect();
            }
        }
    }

    /// <summary>
    /// Manage the colors of all objects.
    /// </summary>
    private void ManageColors()
    {
        filter = Filters.NONE;

        //Check if manager is in CONNECTION_MODE
        if (dragOriginNode != null)
        {
            filter = Filters.CONNECTION_MODE;

            //Check connection afinity
            var allNodes = FindObjectsOfType<NodeController>();

            foreach (var node in allNodes)
            {
                var nodeView = node.GetComponent<NodeView>();

                Product product;

                if (dragOriginNode.GetFreeOutput() != null)
                {
                    product = dragOriginNode.GetFreeOutput().Product;
                }
                else
                {
                    product = null;
                    return;
                }

                switch (dragOriginNode.CanConnectWith(node, product))
                {
                    case 0:
                        break;
                    case 1:
                        nodeView.Paint(nodeView.GetNodeData().color);
                        nodeView.SetBright(Color.green);
                        break;
                    case 2:
                        nodeView.SetBright(Color.magenta);
                        break;
                }
            }
        }
    }

    public void ConnectNodes(NodeController from, NodeController to)
    {
        var connection = (Instantiate(Resources.Load("Nodes/Connection"), null) as GameObject).GetComponent<ConnectionController>();
        connection.Connect(from, to);
    }
}
