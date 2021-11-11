using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConectionView : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer border, color;
    [SerializeField]
    private Transform element;

    [SerializeField]
    private Transform from, to;

    private float speed = 1f;
    private float maxTime = 4f; //seconds
    private float actualTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // follow position
        FollowPositions(from.position,to.position);

        // move element
        MoveElement(from.position, to.position);
    }

    public void MoveElement(Vector3 from, Vector3 to)
    {
        actualTime += Time.deltaTime * speed;
        if (actualTime > maxTime)
        {
            actualTime = 0;
        }
        element.position = Vector3.Lerp(to, from, actualTime / maxTime);
    }

    public void FollowPositions(Vector3 from,Vector3 to)
    {
        this.transform.position = from;
        this.transform.right = to - from;
        //var dt = to.position - from.position;
        var dis = Vector3.Distance(from, to);
        border.size = color.size = new Vector2(dis * 5, border.size.y);
    }
}
