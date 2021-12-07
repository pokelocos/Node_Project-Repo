using System.Collections;
using UnityEngine;
using RA.InputManager;

public class ConnectionController : MonoBehaviour, SelectableObject
{
    [SerializeField] private ConnectionView connectionView;
    [SerializeField] private Gradient fadeGradient;
    private NodeController from, to;

    private int productQueue;
    private Color fadeColor = Color.clear;

    public void Connect(NodeController from, NodeController to)
    {
        var connectionView = GetComponent<ConnectionView>();

        //Variable initialization

        this.from = from;
        this.to = to;

        //Set inputs and outputs
        to.AddInput(new Port(this, from.GetFreeOutput().Product));

        from.SetOutput(this);

        //Events created for both nodes
        connectionView.onConnectionCreated += from.ConnectionUpdated;
        connectionView.onConnectionCreated += to.ConnectionUpdated;

        connectionView.onConnectionDestroyed += from.ConnectionUpdated;
        connectionView.onConnectionDestroyed += to.ConnectionUpdated;

        connectionView.onElementReach += ProductReach;

        //Set points
        connectionView.SetPoints(from.transform, to.transform);
    }

    private void Update()
    {
        if (NodeManager.Filter == NodeManager.Filters.NONE)
        {
            float value = productQueue / 3f;
            fadeColor = Color.Lerp(fadeColor, fadeGradient.Evaluate(value), Time.deltaTime * 3);
            
            connectionView.PaintFade(fadeColor);
        }
    }

    private void ProductReach()
    {
        productQueue++;

        productQueue = Mathf.Clamp(productQueue, 0, 3);
    }

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

    public void Disconnect()
    {
        var connectionView = GetComponent<ConnectionView>();

        to.RemoveInput(this);
        from.RemoveOutput(this);

        connectionView.DestroyConnection();

        Destroy(this.gameObject);
    }
}