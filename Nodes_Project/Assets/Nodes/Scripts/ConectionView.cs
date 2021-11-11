using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConectionView : MonoBehaviour
{
    private NodeView origin;
    private NodeView destination;

    [SerializeField]
    private SpriteRenderer border, color;
    [SerializeField]
    private Transform element;

    private float speed = 1f;
    private float maxTime = 4f; //seconds
    private float actualTime = 0f;

    void Update()
    {
        if (origin != null && destination != null)
        {
            FollowPositions(origin.transform.position, destination.transform.position);
            MoveElement(origin.transform.position, destination.transform.position);
        }
        else
        {
            element.gameObject.SetActive(false);
        }
    }

    public void SetNodes(NodeView origin, NodeView dest)
    {
        this.origin = origin;
        this.destination = dest;
    }

    public void MoveElement(Vector3 from, Vector3 to)
    {
        element.gameObject.SetActive(true);

        actualTime += Time.deltaTime * speed;
        if (actualTime > maxTime)
        {
            actualTime = 0;
        }
        element.position = Vector3.Lerp(from, to, actualTime / maxTime);
    }

    public void FollowPositions(Vector3 from, Vector3 to)
    {
        this.transform.position = from;
        this.transform.right = to - from;
        var dis = Vector3.Distance(from, to);
        border.size = color.size = new Vector2(dis * 5, border.size.y);
    }
}
