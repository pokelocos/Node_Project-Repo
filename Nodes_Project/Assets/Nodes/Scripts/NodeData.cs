using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New node data",menuName = "Node game.../Create node data")]
public class NodeData : ScriptableObject
{
    public Color color = Color.gray;
    public Sprite icon;
    public float speed = 1f;
    public float productionTime = 5f;
}
