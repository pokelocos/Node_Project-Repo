using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using RA.InputManager;
using RA.Generic;

public class NodeManager : MonoBehaviour // change name (?) -> node (inputs?)(interactions?)(actions??)
{
    [Header("Settings")]
    [SerializeField] private Filters filter = Filters.NONE;

    //Visible variables
    [Header("Components")]
    private ConnectionView proxyConnection;
    [SerializeField] private ConnectionView ErrorConnection;
    [SerializeField] private ConnectionView normalConnection;
    [SerializeField] private IngredientProxyView ingredientProxyView;

    //Hidden variables
    private InputManager inputManager;
    private NodeController dragOriginNode;
    private static NodeManager _singleton; //!!!

    //private NodeController targetNode;

    [Header("References")]
    [SerializeField] private NodeInformationView informationView;

    private float lastClickTime;
    private const float DOUBLE_CLICK_TIME = .2f;

    [SerializeField] private NodeConnectionDisplay connectionDisplay;

    public AudioSource source;
    public AudioClip cannotConect;
    public AudioClip canConect;
    public AudioClip nothingConect;


    public delegate void ConectionEvent(NodeController n1,NodeController n2);
    public ConectionEvent OnConnect;

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

    private void Awake()
    {
        inputManager = FindObjectOfType<InputManager>();
    }

    private void Start()
    {
        proxyConnection = normalConnection;
        proxyConnection.gameObject.SetActive(false);
    }

    void Update()
    {
        // Double Right Click -> show "NodeView"  or disconect connection
        if (inputManager.RightDoubleClick)
        {
            var overObj = inputManager.OverObject;
            if (overObj is ConnectionController)
            {
                DisconnectConnection(overObj as ConnectionController);
            }
            else if (overObj is NodeController)
            {
                DisplayNodeInformation(overObj as NodeController);
            }
        }

        // Right click -> Drag connection
        if (Input.GetMouseButtonDown(1) && inputManager.OverObject is NodeController)
        {
            OnDragConnectionStart();
        }
        else if (Input.GetMouseButton(1) && dragOriginNode != null)
        {
            OnDragConnectionStay();
        }
        else if (Input.GetMouseButtonUp(1) && dragOriginNode != null)
        {
            OnDragConnectionEnd();
        }

        //ManageColors();
    }    

    private void DisconnectConnection(ConnectionController connection)
    {
        connection.Disconnect();
    }

    /// <summary>
    /// Show a panel wiht the information of the node.
    /// </summary>
    /// <param name="target"></param>
    private void DisplayNodeInformation(NodeController target) // Double Right Click -> show "NodeView" 
    {
        informationView.DisplayInformation(target);
    }

    /// <summary>
    /// Obtains an available port and activates the proxy connection
    /// with the corresponding ingredient.
    /// </summary>
    private void OnDragConnectionStart() 
    {
        var node = inputManager.OverObject as NodeController;
        var freePort = node.GetFreeOutput();

        if (freePort != null)
        {
            proxyConnection = normalConnection;
            ingredientProxyView.ShowInformation(filter, freePort.Product);
        }
        else
        {
            proxyConnection = ErrorConnection;
        }

        dragOriginNode = inputManager.OverObject as NodeController;
        proxyConnection.gameObject.SetActive(true);

        Vector3 from = dragOriginNode.transform.position;
        Vector3 to = inputManager.MouseWorldPosition;
        proxyConnection.FollowPositions(from, to);
    }

    /// <summary>
    /// Keep the proxy connection following the pointer.
    /// </summary>
    private void OnDragConnectionStay()
    {
        Vector3 from = dragOriginNode.transform.position;
        Vector3 to = inputManager.MouseWorldPosition;

        proxyConnection.FollowPositions(from, to);

        if (dragOriginNode.GetFreeOutput() != null)
        {
            proxyConnection.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Connects the origin node and the node below the pointer,
    /// if the connection is not valid or there is no node below the pointer, it does nothing.
    /// besides, the proxy connection is activated.
    /// </summary>
    private void OnDragConnectionEnd()
    {
        var overObj = (NodeController)inputManager.OverObject;
        if (inputManager.OverObject is NodeController)
        {
            var originPort = dragOriginNode.GetFreeOutput();
            if (overObj &&
                originPort != null &&
                dragOriginNode.CanConnectWith(overObj, originPort.Product) == 1)
            {
                ConnectNodes(dragOriginNode, overObj);
                OnConnect?.Invoke(dragOriginNode,overObj);
            }
        }

        dragOriginNode = null;
        proxyConnection.gameObject.SetActive(false);
        ingredientProxyView.HidenInformation();
    }


    private void ManageColors() // List<NodeController> nodes
    {
        /*
        //Repaint connection proxy in gray
        proxyConnection.Paint(Color.gray, Color.gray.Darker());

        filter = Filters.NONE;

        //Check if manager is in CONNECTION_MODE
        if (dragOriginNode != null)
        {
            filter = Filters.CONNECTION_MODE;

            //Check connection afinity
             var allNodes = FindObjectsOfType<NodeController>();
            //var allNodes = nodes;

            foreach (var node in allNodes)
            {
                var nodeView = node.NodeView;

                Product product;

                if (dragOriginNode.GetFreeOutput() != null)
                {
                    product = dragOriginNode.GetFreeOutput().Product;
                }
                else
                {
                    product = null;

                    if (node == targetNode)
                    {
                        proxyConnection.Paint(Color.red, Color.red.Darker());
                    }
                    return;
                }

                int value = dragOriginNode.CanConnectWith(node, product);

                switch (value)
                {
                    case 0:

                        if (node == targetNode)
                        {
                            proxyConnection.Paint(Color.red, Color.red.Darker());
                        }

                        break;
                    case 1:
                        nodeView.SetBodyColor(node.GetData().color);

                        if (node == targetNode)
                        {
                            proxyConnection.Paint(Color.green, Color.green.Darker());
                        }

                        break;
                    case 2:

                        foreach (var inputPort in node.GetInputPorts().Where(x => x.connection != null))
                        {
                            inputPort.connection.ShowAlert(1, ConnectionController.AlertType.CROSS, new Color(1, 0.2877358f, 0.2877358f, 1));
                        }

                        if (node == targetNode)
                        {
                            proxyConnection.Paint(Color.red, Color.red.Darker());
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

                        if (node == targetNode)
                        {
                            proxyConnection.Paint(Color.red, Color.red.Darker());
                        }
                        break;
                }
            }
        }
        */
    }

    public static void ConnectNodes(NodeController from, NodeController to) // funcion bosoleta (??)
    {
        var connection = (Instantiate(Resources.Load("Connection"), null) as GameObject).GetComponent<ConnectionController>();
        connection.Connect(from, to);
    }
}
