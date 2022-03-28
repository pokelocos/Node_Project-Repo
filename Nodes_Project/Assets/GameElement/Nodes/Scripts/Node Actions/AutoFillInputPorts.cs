using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "RA/Nodes/Actions/Auto-fill input ports")]
public class AutoFillInputPorts : NodeActions
{
    public override void CallAction(NodeController node)
    {
        foreach (var input in node.GetInputPorts())
        {
            input.isProductInPort = true;
        }
    }
}