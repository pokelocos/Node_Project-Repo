using System.Collections;
using UnityEngine;
using System;

[Obsolete]
public abstract class NodeActions : ScriptableObject
{
    public abstract void CallAction(NodeController node);
}