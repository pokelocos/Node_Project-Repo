using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NodeView : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer fillBar;
    [SerializeField]
    private NodeData data;

    private float maxTime = 4f; //seconds
    private float actualTime = 0f;
    protected float internalSpeed = 0;
    void Update()
    {
        Work();
    }

    private void Work()
    {
        actualTime += Time.deltaTime * data.speed * internalSpeed;
        if (actualTime > data.productionTime)
        {
            actualTime = 0;
            OnWorkFinish();
        }

        var radial = (1 - (actualTime / data.productionTime)) * 360;
        fillBar.GetComponent<SpriteRenderer>().material.SetFloat("_Arc2", radial);
    }

    protected abstract void OnWorkFinish();
}
