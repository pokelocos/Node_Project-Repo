using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConectionView : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer border, color;

    [SerializeField]
    private Transform from, to;

    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = from.position;
        this.transform.right = to.position - from.position;
        //var dt = to.position - from.position;
        var dis = Vector3.Distance(from.position,to.position);
        border.size = color.size = new Vector2(dis * 5,border.size.y);
    }
}
