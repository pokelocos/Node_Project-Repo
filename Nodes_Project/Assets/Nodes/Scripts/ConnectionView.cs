using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionView : MonoBehaviour
{
    private NodeView origin;
    private NodeView destination;

    private Transform from;
    private Transform to;

    [SerializeField]
    private SpriteRenderer border, color, feedback;
    [SerializeField]
    private SpriteRenderer element;

    public Color border_color;
    public Color body_color;

    public delegate void ConnectionCreated(ConnectionView connection);
    public delegate void ConnectionDestroyed(ConnectionView connection);

    public event ConnectionCreated onConnectionCreated;
    public event ConnectionDestroyed onConnectionDestroyed;

    private int hasIngredient;
    public bool isReadyToClaim;
    
    private float speed = 1f;
    private float maxTime = 4f; //seconds
    private float actualTime = 0f;

    Ingredient currentIngredient;

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

                Color darker = Color.Lerp(Color.gray, Color.black, 0.2f);

                Paint(Color.gray, darker);
                break;
        }

        if (from != null && to != null)
        {
            FollowPositions(from.position, to.position);
            //MoveElement(from.position, to.position);
        }
        else
        {
            element.gameObject.SetActive(false);
        }
    }

    public Vector3 GetMiddlePoint()
    {
        return (from.position + to.position) / 2;
    }

    public Ingredient GetOutputIngredient()
    {
        return currentIngredient;
    }

    public Ingredient GetIngredient()
    {
        Ingredient result = null;

        for (int i = 0; i < origin.GetOutputs().Length; i++)
        {
            if (origin.GetOutputs()[i] != null && origin.GetOutputs()[i] == this)
            {
                if (origin.GetCurrentRecipe() != null)
                {
                    result = origin.GetCurrentRecipe().GetOutputs()[i];
                }
            }
        }

        return result;
    }

    [System.Obsolete("This is an obsolete method")]
    public void Disconnect()
    {
        origin.RemoveConnection(this);
        destination.RemoveConnection(this);

        origin.ConnectionChange();
        destination.ConnectionChange();

        Destroy(gameObject);
    }

    public void Paint(Color body, Color border)
    {
        this.border.color = border;
        this.color.color = body;
    }

    public void SetDotColor(Color color)
    {
        element.color = color;
    }

    [System.Obsolete("This is an obsolete method")]
    public void SetNodes(NodeView origin, NodeView dest)
    {
        this.origin = origin;
        this.destination = dest;

        this.origin.ConnectionChange();
        this.destination.ConnectionChange();

        if (GetIngredient() != null)
        {
            SetDotColor(GetIngredient().color);
        }
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

        Destroy(this.gameObject);
    }

    public NodeView GetOrigin()
    {
        return origin;
    }

    public NodeView GetDestination()
    {
        return destination;
    }

    public void SendIngredient(Ingredient ingredient)
    {
        currentIngredient = ingredient;
        hasIngredient = 1;
    }

    public void MoveElement(Vector3 from, Vector3 to)
    {
        element.gameObject.SetActive(true);

        actualTime += Time.deltaTime * speed * hasIngredient;

        if (actualTime > maxTime)
        {
            if (isReadyToClaim)
            {
                GetComponentInChildren<Animator>().SetTrigger("Fail");
            }
            else
            {
                GetComponentInChildren<Animator>().SetTrigger("Success");
            }

            actualTime = 0;
            hasIngredient = 0;
            isReadyToClaim = true;

            destination.InputIngredientReady(this);
        }

        element.transform.position = Vector3.Lerp(from, to, actualTime / maxTime);
    }

    public void FollowPositions(Vector3 from, Vector3 to)
    {
        this.transform.position = from;
        this.transform.right = to - from;
        var dis = Vector3.Distance(from, to);
        border.size = color.size = new Vector2(dis * 5, border.size.y);

        feedback.size = border.size;

        UpdateBoxCollider(gameObject.GetComponent<BoxCollider2D>(), border.sprite);
    }
    
    private void UpdateBoxCollider(BoxCollider2D collider, Sprite newSprite)
    {
        collider.size = border.size;
        collider.offset = new Vector2(border.size.x / 2, collider.offset.y);
    }
}
