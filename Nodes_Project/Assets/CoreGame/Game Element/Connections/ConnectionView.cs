using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionView : MonoBehaviour
{
    

    [SerializeField] private SpriteRenderer border, color, input_icon, output_icon;
    [SerializeField] private GameObject fade;

    // element -> to class "TwoColorView" or "IngredientView"
    [SerializeField] private SpriteRenderer elementColor;
    [SerializeField] private SpriteRenderer elementBorder;

    [SerializeField] private Color borderColor;
    [SerializeField] private Color bodyColor;

    private Transform from;
    private Transform to;

    

    private void Start()
    {
        SetColor(bodyColor, borderColor);
    }

    void Update()
    {
        switch (NodeManager.Filter)
        {
            case NodeManager.Filters.NONE:
                SetColor(bodyColor, borderColor);
                HideIcons(0);
                break;
            case NodeManager.Filters.CONNECTION_MODE:
                PaintFade(Color.clear);
                Color darker = Color.Lerp(Color.gray, Color.black, 0.2f);
                SetColor(Color.gray, darker);
                break;
        }


        if (from != null && to != null)
        {
            FollowPositions(from.position, to.position);
        }
        /*
            if (isMovingElement)
            {
                MoveElement(from.position, to.position);
            }
        }
        else
        {
            element.gameObject.SetActive(false);
        }
        */
    }

    public Vector2 Size()
    {
        return border.size;
    }

    public void SetFollowingPoints(Transform from, Transform to)
    {
        this.from = from;
        this.to = to;

        //onConnectionCreated?.Invoke(this);
    }

    /// <summary>
    /// Display an icon in the ends of the connections. -1 = Input side. 1 = Output side. 0 = Both sides.
    /// </summary>
    /// <param name="side"></param>
    /// <param name="alertType"></param>
    /// <param name="color"></param>
    public void ShowIcons(int side, Sprite sprite, Color color)
    {
        if (side < 0)
        {
            input_icon.color = color;
            input_icon.sprite = sprite;
        }

        if (side == 0)
        {
            input_icon.color = color;
            input_icon.sprite = sprite;

            output_icon.color = color;
            output_icon.sprite = sprite;
        }

        if (side > 0)
        {
            output_icon.color = color;
            output_icon.sprite = sprite;
        }
    }

    /// <summary>
    /// Hide an icon in the ends of the connections. -1 = Input side. 1 = Output side. 0 = Both sides.
    /// </summary>
    /// <param name="side"></param>
    public void HideIcons(int side)
    {
        if (side < 0)
        {
            input_icon.color = Color.clear;
        }

        if (side == 0)
        {
            input_icon.color = Color.clear;

            output_icon.color = Color.clear;
        }

        if (side > 0)
        {
            output_icon.color = Color.clear;
        }
    }

    public void SendElement()
    {
        //isMovingElement = true;
    }

    //public Vector3 GetMiddlePoint()
    //{
    //    return (from.position + to.position) / 2;
    //}

    public void SetColor(Color body, Color border)
    {
        this.border.color = border;
        this.color.color = body;
    }

    public void PaintFade(Color color)
    {
        fade.GetComponentInChildren<SpriteRenderer>().color = color;
    }

    public void SetDotColor(Color c1, Color c2)
    {
        elementColor.color = c1;
        elementBorder.color = c2;
    }


    public void DestroyConnection() // eliminar ???
    {
        //onConnectionDestroyed?.Invoke(this);
    }

    /// <summary>
    /// Set ingretient position between the nodes following
    /// </summary>
    /// <param name="value"></param>
    public void SetElementPosition(float value)
    {
        elementColor.transform.position = Vector3.Lerp(from.position, to.position, value);
    }

    public void SetElementPosition(Vector3 from, Vector3 to, float value)
    {
        elementColor.transform.position = Vector3.Lerp(from,to,value);
    }

    public void FollowPositions(Vector3 from, Vector3 to)
    {
        this.transform.position = from;
        this.transform.right = to - from;
        var dis = Vector3.Distance(from, to);
        border.size = color.size = new Vector2(dis * 5, border.size.y);

        fade.transform.position = to;
        input_icon.transform.position = from;
        output_icon.transform.position = to;

        fade.transform.localPosition -= Vector3.right * 8;

        input_icon.transform.localPosition += Vector3.right * 8;
        output_icon.transform.localPosition -= Vector3.right * 8;
    }
}
