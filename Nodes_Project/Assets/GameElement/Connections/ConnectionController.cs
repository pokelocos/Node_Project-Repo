using System.Collections;
using UnityEngine;
using RA.InputManager;

public class ConnectionController : MonoBehaviour, SelectableObject
{
    [Header("Pref references")]
    [SerializeField] private new BoxCollider2D collider;
    [SerializeField] private ConnectionView connectionView;

    [Header("Variables")]
    [SerializeField] private Gradient fadeGradient;

    private NodeController from, to;
    private Port inputPort;

    private int productQueue;
    private Color fadeColor = Color.clear;

    [SerializeField] private Sprite[] alert_sprites;

    public delegate void ConnectionEvent(ConnectionController connection);
    public event ConnectionEvent onConnectionCreated;
    public event ConnectionEvent onConnectionDestroyed;
    public event ConnectionEvent onElementReach;

    public delegate void ProductReceive(Port port); // "ProductRecive" -> "PortEvent" (?)
    public event ProductReceive onProductReceive;

    private float speed = 1f;
    private float maxTime = 4f; //seconds
    private float actualTime = 0f;

    private bool isMovingElement;

    public enum AlertType
    {
        CROSS, TRIANGLE, CIRCLE, DIAMOND
    }

    public  ConnectionView View { get { return connectionView; } } 

    public void Connect(NodeController from, NodeController to)
    {
        var connectionView = GetComponent<ConnectionView>(); // quitar en su momento

        //Variable initialization
        this.from = from;
        this.to = to;

        //Set inputs and outputs
        inputPort = new Port(this, from.GetFreeOutput().Product);
        to.AddInput(inputPort);

        from.SetOutput(this);

        //Events created for both nodes
        onConnectionCreated += from.ConnectionUpdated;
        onConnectionCreated += to.ConnectionUpdated;

        onConnectionDestroyed += from.ConnectionUpdated;
        onConnectionDestroyed += to.ConnectionUpdated;

        onElementReach += ProductReach; //?
        onProductReceive += to.OnInputPortReceiveProduct;

        //Set points
        connectionView.SetFollowingPoints(from.transform, to.transform);
        onConnectionCreated?.Invoke(this);
    }

    private void Update()
    {
        if (NodeManager.Filter == NodeManager.Filters.NONE)
        {
            fadeColor = GetQueueColor();
            connectionView.PaintFade(fadeColor);
        }

        UpdateBoxCollider(collider,View.Size());

        if(isMovingElement)
        {
            UpdateElement();
        }
    }

    public void UpdateElement()
    {
        if (actualTime > maxTime)
        {
            actualTime = 0;
            isMovingElement = false;
            onElementReach?.Invoke(this);
        }
        else
        {
            View.SetElementPosition(actualTime / maxTime);
            actualTime += Time.deltaTime;
        }
    }

    private void UpdateBoxCollider(BoxCollider2D collider,Vector2 size)
    {
        collider.size = size;
        collider.offset = new Vector2(size.x / 2, collider.offset.y);
    }

    public Color GetQueueColor()
    {
        float value = productQueue / 3f;
        return Color.Lerp(fadeColor, fadeGradient.Evaluate(value), Time.deltaTime * 3);
    }

    /// <summary>
    /// Called when this connection element reach the end of the line.
    /// </summary>
    private void ProductReach(ConnectionController cc)
    {
        productQueue++;
        onProductReceive?.Invoke(inputPort);
    }

    /// <summary>
    /// Reset the counter of times that this connetion alredy receive a product.
    /// </summary>
    public void UseConnection()
    {
        productQueue = 0;
    }

    public Product GetProduct()
    {
        return GetOrigin().GetOutputPort(this).Product;
    }

    public NodeController GetOrigin()
    {
        return from;
    }

    public NodeController GetDestination()
    {
        return to;
    }

    public void SendProduct()
    {
        isMovingElement = true;
        connectionView.SendElement();
    }

    /// <summary>
    /// Display an alert in the ends of the connections. -1 = Input side. 1 = Output side. 0 = Both sides.
    /// </summary>
    /// <param name="side"></param>
    /// <param name="alertType"></param>
    /// <param name="color"></param>
    public void ShowAlert(int side, AlertType alertType, Color color)
    {
        connectionView.ShowIcons(side, alert_sprites[(int)alertType], color);
    }

    /// <summary>
    /// Hide an alert in the ends of the connections. -1 = Input side. 1 = Output side. 0 = Both sides.
    /// </summary>
    /// <param name="side"></param>
    public void HideAlerts(int side)
    {
        connectionView.HideIcons(side);
    }

    public void Disconnect()
    {
        to.RemoveInput(this);
        from.RemoveOutput(this);

        connectionView.DestroyConnection();

        Destroy(this.gameObject);
    }
}