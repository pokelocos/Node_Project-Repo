using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeView : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer fillBar;

    public float speed = 1f;

    private float maxTime = 4f; //seconds
    private float actualTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        actualTime += Time.deltaTime * speed;
        if(actualTime > maxTime)
        {
            actualTime = 0;
            // do event
        }

        var radial = (1 -(actualTime / maxTime)) * 360;
        fillBar.GetComponent<SpriteRenderer>().material.SetFloat("_Arc2", radial);
    }
}
