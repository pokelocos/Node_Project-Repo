using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionView : MonoBehaviour
{
    private Transform from;
    private Transform to;

    [SerializeField]
    private SpriteRenderer border, color;
    [SerializeField] private GameObject fade;
    [SerializeField]
    private SpriteRenderer element;

    public Color border_color;
    public Color body_color;

    public delegate void ConnectionCreated(ConnectionView connection);
    public delegate void ConnectionDestroyed(ConnectionView connection);
    public delegate void ElementReach();

    public event ConnectionCreated onConnectionCreated;
    public event ConnectionDestroyed onConnectionDestroyed;
    public event ElementReach onElementReach;

    private bool isMovingElement;

    private float speed = 1f;
    private float maxTime = 4f; //seconds
    private float actualTime = 0f;

    private void Start()
    {
        Paint(body_color, border_color);
    }

    void Update()
    {
        switch (NodeManager.Filter)
        {
            case NodeManager.Filters.NONE:
                Paint(body_color, border_color);
                
                break;
            case NodeManager.Filters.CONNECTION_MODE:
                PaintFade(Color.clear);
                Color darker = Color.Lerp(Color.gray, Color.black, 0.2f);
                Paint(Color.gray, darker);
                break;
        }

        if (from != null && to != null)
        {
            FollowPositions(from.position, to.position);

            if (isMovingElement)
            {
                MoveElement(from.position, to.position);
            }
        }
        else
        {
            element.gameObject.SetActive(false);
        }
    }

    public void SendElement()
    {
        isMovingElement = true;
    }

    public Vector3 GetMiddlePoint()
    {
        return (from.position + to.position) / 2;
    }

    public void Paint(Color body, Color border)
    {
        this.border.color = border;
        this.color.color = body;
    }

    public void PaintFade(Color color)
    {
        fade.GetComponentInChildren<SpriteRenderer>().color = color;
    }

    public void SetDotColor(Color color)
    {
        element.color = color;
    }

    public void SetPoints(Transform from, Transform to)
    {
        this.from = from;
        this.to = to;

        onConnectionCreated?.Invoke(this);
    }

    //Destroy the connection objects and send onConnectionDestroyed event
    public void DestroyConnection()
    {
        onConnectionDestroyed?.Invoke(this);
    }

    public void MoveElement(Vector3 from, Vector3 to)
    {
        element.gameObject.SetActive(true);

        actualTime += Time.deltaTime * speed;

        if (actualTime > maxTime)
        {
            actualTime = 0;
            isMovingElement = false;
            onElementReach?.Invoke();
        }

        element.transform.position = Vector3.Lerp(from, to, actualTime / maxTime);
    }

    public void FollowPositions(Vector3 from, Vector3 to)
    {
        this.transform.position = from;
        this.transform.right = to - from;
        var dis = Vector3.Distance(from, to);
        border.size = color.size = new Vector2(dis * 5, border.size.y);

        fade.transform.position = to;

        fade.transform.localPosition -= Vector3.right * 8;

        UpdateBoxCollider(gameObject.GetComponent<BoxCollider2D>(), border.sprite);
    }
    
    private void UpdateBoxCollider(BoxCollider2D collider, Sprite newSprite)
    {
        collider.size = border.size;
        collider.offset = new Vector2(border.size.x / 2, collider.offset.y);
    }
}
