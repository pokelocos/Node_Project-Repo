using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condicion : MonoBehaviour
{
    public int minInput;
    public int minOutputs;

    private bool CheckCondition(NodeView node)
    {
        return (node.GetOutputs().Length > minInput) && 
            (node.GetOutputs().Length > minOutputs);
    }

}
