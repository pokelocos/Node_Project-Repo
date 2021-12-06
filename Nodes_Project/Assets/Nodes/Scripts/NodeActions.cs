using System.Collections;
using UnityEngine;

public abstract class NodeActions : ScriptableObject
{
    public abstract void CallAction(NodeController node);
}