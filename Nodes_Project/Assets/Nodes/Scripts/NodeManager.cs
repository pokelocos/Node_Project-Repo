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

    [Header("References")]
    [SerializeField]
    private SpriteRenderer icon_handler;
    [SerializeField]
    private SpriteRenderer icon_background_handler;
    [SerializeField]
    private GameObject price_shadow_handler;
    [SerializeField]
    private TextMesh price_handler;
    [SerializeField]
    private Vector3 hanlder_offset = new Vector3(0.5f, 1, 0);

    [SerializeField]
    private Sprite invalidIngredient;

    private float lastClickTime;
    private const float DOUBLE_CLICK_TIME = .2f;

    [SerializeField]
    private NodeConnectionDisplay connectionDisplay;

    [SerializeField]

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

        ManageHanlder();
    }    

    /// <summary>
    /// Controls the icons from the mouse
    /// </summary>
    private void ManageHanlder()
    {
        icon_handler.sprite = null;
        price_handler.text = string.Empty;

        icon_handler.transform.position = inputManager.MouseWorldPosition + hanlder_offset;

        icon_background_handler.gameObject.SetActive(false);
        price_shadow_handler.gameObject.SetActive(false);

        if (filter == Filters.CONNECTION_MODE)
        {
            var output = dragOriginNode.GetFreeOutput();

            icon_background_handler.gameObject.SetActive(true);
            price_shadow_handler.gameObject.SetActive(false);

            if (output != null)
            {
                icon_handler.sprite = output.Product.data.icon;
                price_handler.text = "$" + output.Product.currentValue;
            }
            else
            {
                icon_handler.sprite = invalidIngredient;
            }
        }
    }

    /// <summary>
    /// Controls the logic of the connections.
    /// </summary>
    private void ManageConnections()
    {
        proxyConnection.gameObject.SetActive(false);

        if (inputManager.OverObject is NodeController)
        {
            targetNode = inputManager.OverObject as NodeController;
        }
        else
        {
            targetNode = null;
        }

        // Proxy connection
        if (dragOriginNode != null)
        {
            Vector3 from = dragOriginNode.transform.position;
            Vector3 to = inputManager.MouseWorldPosition;

            proxyConnection.gameObject.SetActive(true);

            proxyConnection.FollowPositions(from, to);

            if (dragOriginNode.GetFreeOutput() != null)
            {
                proxyConnection.gameObject.SetActive(true);
            }
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
                        break;
                    case 2:

                        foreach (var inputPort in node.GetInputPorts().Where(x => x.connection != null))
                        {
                            inputPort.connection.ShowAlert(1, ConnectionController.AlertType.CROSS, new Color(1, 0.2877358f, 0.2877358f, 1));
                        }

                        break;
                    case 3:
                        foreach (var outputPort in dragOriginNode.GetOutputPorts())
                        {
                            if (outputPort.connection != null && outputPort.connection.GetDestination() == node)
                            {
                                outputPort.connection.ShowAlert(1, ConnectionController.AlertType.TRIANGLE, new Color(1, 0.2877358f, 0.2877358f, 1));
                                break;
                            }
                        }
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
