using System.Collections;
using UnityEngine;
using RA.InputManager;

public class ConnectionController : MonoBehaviour, SelectableObject
{
    [SerializeField] private ConnectionView connectionView;
    [SerializeField] private Gradient fadeGradient;
    private NodeController from, to;
    private Port inputPort;

    public delegate void ProductReceive(Port port);
    public event ProductReceive onProductReceive;

    private int productQueue;
    private Color fadeColor = Color.clear;

    [SerializeField] private Sprite[] alert_sprites;

    public enum AlertType
    {
        CROSS, TRIANGLE, CIRCLE, DIAMOND
    }

    public  ConnectionView View { get { return connectionView; } } 

    public void Connect(NodeController from, NodeController to)
    {
        var connectionView = GetComponent<ConnectionView>();

        //Variable initialization

        this.from = from;
        this.to = to;

        //Set inputs and outputs
        inputPort = new Port(this, from.GetFreeOutput().Product);
        to.AddInput(inputPort);

        from.SetOutput(this);

        //Events created for both nodes
        connectionView.onConnectionCreated += from.ConnectionUpdated;
        connectionView.onConnectionCreated += to.ConnectionUpdated;

        connectionView.onConnectionDestroyed += from.ConnectionUpdated;
        connectionView.onConnectionDestroyed += to.ConnectionUpdated;

        connectionView.onElementReach += ProductReach;
        onProductReceive += to.OnInputPortReceiveProduct;

        //Set points
        connectionView.SetPoints(from.transform, to.transform);
    }

    private void Update()
    {
        if (NodeManager.Filter == NodeManager.Filters.NONE)
        {
            fadeColor = GetQueueColor();


            connectionView.PaintFade(fadeColor);
        }
    }

    public Color GetQueueColor()
    {
        float value = productQueue / 3f;
        return Color.Lerp(fadeColor, fadeGradient.Evaluate(value), Time.deltaTime * 3);
    }

    /// <summary>
    /// Called when this connection element reach the end of the line.
    /// </summary>
    private void ProductReach()
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
        var connectionView = GetComponent<ConnectionView>();

        to.RemoveInput(this);
        from.RemoveOutput(this);

        connectionView.DestroyConnection();

        Destroy(this.gameObject);
    }
}