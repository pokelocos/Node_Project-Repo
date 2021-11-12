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

    private int hasIngredient;
    public bool isReadyToClaim;
    
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

    public void SetNodes(NodeView origin, NodeView dest)
    {
        this.origin = origin;
        this.destination = dest;
    }

    public NodeView GetOrigin()
    {
        return origin;
    }

    public NodeView GetDestination()
    {
        return destination;
    }

    public void SendIngredient()
    {
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
                //Mostrar advertencia de perdida de ingrediente
            }

            actualTime = 0;
            hasIngredient = 0;
            isReadyToClaim = true;

            destination.InputIngredientReady(this);
        }

        element.position = Vector3.Lerp(from, to, actualTime / maxTime);
    }

    public void FollowPositions(Vector3 from, Vector3 to)
    {
        this.transform.position = from;
        this.transform.right = to - from;
        var dis = Vector3.Distance(from, to);
        border.size = color.size = new Vector2(dis * 5, border.size.y);

        UpdateBoxCollider(gameObject.GetComponent<BoxCollider2D>(), border.sprite);
    }
    
    private void UpdateBoxCollider(BoxCollider2D collider, Sprite newSprite)
    {
        collider.size = border.size;
        collider.offset = new Vector2(border.size.x / 2, collider.offset.y);
    }
}
