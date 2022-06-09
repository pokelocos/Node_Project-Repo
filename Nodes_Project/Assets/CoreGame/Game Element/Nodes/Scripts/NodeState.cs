using MicroFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public struct NodeState
{
    public string dataName;
    [SerializeField] public Tuple<float,float> position;
    public float currentTime;

    public Vector2 Position
    {
        get
        {
            return new Vector2(position.Item1,position.Item2);
        }
    }

    public NodeState (string dataName, float x,float y, float currentTime)
    {
        this.dataName = dataName;
        this.position = new Tuple<float,float>(x,y);
        this.currentTime = currentTime;
    }
}

[Serializable]
public struct EffectState
{
    public string effectName; 
    public float currentTime;

    public EffectState(string effectName, float currentTime)
    {
        this.effectName = effectName;
        this.currentTime = currentTime;
    }
}

[Serializable]
public struct ConnectionState
{
    public Tuple<int, int> nodeRelationIndex;
    public string ingredientName;

    public ConnectionState(Tuple<int, int> nodeRelationIndex, string ingredientName)
    {
        this.nodeRelationIndex = nodeRelationIndex;
        this.ingredientName = ingredientName;
    }

    public ConnectionState(int n1,int n2, string ingredientName)
    {
        this.nodeRelationIndex = new Tuple<int,int>(n1,n2);
        this.ingredientName = ingredientName;
    }
}

