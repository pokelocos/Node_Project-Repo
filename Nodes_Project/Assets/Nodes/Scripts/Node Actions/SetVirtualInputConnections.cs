using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "RA/Nodes/Actions/Set virtual inputCconnections")]
public class SetVirtualInputConnections : NodeActions
{
    public override void CallAction(NodeController node)
    {
        foreach (var input in node.GetInputPorts())
        {
            var connection = (Instantiate(Resources.Load("Nodes/Connection"), null) as GameObject).GetComponent<ConnectionController>();
            connection.GetComponent<ConnectionView>().SetPoints(node.transform, node.transform);
            input.connection = connection;
        }
    }
}