using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condicion : MonoBehaviour
{
    public NodeView node;
    public int minInput;
    public int minOutputs;

    private bool CheckCondition()
    {
        return (node.GetOutputs().Length > minInput) && 
            (node.GetOutputs().Length > minOutputs);
    }

}
