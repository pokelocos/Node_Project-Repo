using System.Collections;
using UnityEngine;
using RA.InputManager;

public class ConnectionController : MonoBehaviour, SelectableObject
{
    [SerializeField] private ConnectionView connectionView;

    private NodeController from, to;
    public void Connect(NodeController from, NodeController to)
    {
        var connectionView = GetComponent<ConnectionView>();

        //Variable initialization
        this.from = from;
        this.to = to;

        //Events created for both nodes
        connectionView.onConnectionCreated += from.ConnectionUpdated;
        connectionView.onConnectionCreated += to.ConnectionUpdated;

        connectionView.onConnectionDestroyed += from.ConnectionUpdated;
        connectionView.onConnectionDestroyed += to.ConnectionUpdated;

        //Set points
        connectionView.SetPoints(from.transform, to.transform);
    }

    public void Disconnect()
    {
        var connectionView = GetComponent<ConnectionView>();

        connectionView.DestroyConnection();
    }
}