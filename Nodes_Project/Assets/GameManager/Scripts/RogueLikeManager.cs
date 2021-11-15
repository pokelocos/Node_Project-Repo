using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RogueLikeManager : MonoBehaviour
{
    public void Spawn()
    {
        if ((GameManager.Days + 1) % 3 == 0)
        {
            var nodes = Resources.LoadAll("Node Prefabs");

            Instantiate(nodes[Random.Range(0, nodes.Length - 1)]) ;
        }
    }
}
