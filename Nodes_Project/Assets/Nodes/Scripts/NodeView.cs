using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeView : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer fillBar;
    private NodeData data;

    //public float speed = 1f;

    private float maxTime = 4f; //seconds
    private float actualTime = 0f;

    public delegate void nodeEvent(NodeData data);
    public nodeEvent eventtt; // nombre cuestionable

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        actualTime += Time.deltaTime * data.speed;
        if(actualTime > maxTime)
        {
            actualTime = 0;
            eventtt?.Invoke(data);
        }

        var radial = (1 -(actualTime / maxTime)) * 360;
        fillBar.GetComponent<SpriteRenderer>().material.SetFloat("_Arc2", radial);
    }
}
