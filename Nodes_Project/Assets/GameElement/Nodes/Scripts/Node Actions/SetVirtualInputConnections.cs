using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "RA/Nodes/Actions/Set virtual inputCconnections")]
public class SetVirtualInputConnections : NodeActions
{
    public override void CallAction(NodeController node)
    {
        foreach (var input in node.GetInputPorts()) // el nodo deveria estar creando la connecion (???)
        {
            var resource = Instantiate(Resources.Load("Connection"));
            var connection = (resource as GameObject).GetComponent<ConnectionController>();
            connection.View.SetPoints(node.transform, node.transform);
            input.connection = connection;
        }
    }
}