using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeView : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer body;
    [SerializeField]
    protected SpriteRenderer nodeIcon;
    [SerializeField]
    private SpriteRenderer fillBar;
    [SerializeField]
    private SpriteRenderer bright;
    [SerializeField] private TextMesh misc_text;
    [SerializeField] private TextMesh misc_shadow_text;
    [SerializeField] private Animator misc_text_animator;
    protected NodeData data;

    public void SetView(NodeData nodeData)
    {
        data = nodeData;
        body.color = nodeData.color;
        nodeIcon.sprite = nodeData.icon;
    }

    void Update()
    {
        switch (NodeManager.Filter)
        {
            case NodeManager.Filters.NONE:
                SetBodyColor(data.color);
                SetBright(Color.clear);
                break;
            case NodeManager.Filters.CONNECTION_MODE:
                SetBodyColor(Color.gray);
                break;
        }  
    }

    public void SetBodyColor(Color color)
    {
        body.color = color;
    }

    public void SetBright(Color color)
    {
        if (color == Color.clear)
        {
            bright.gameObject.SetActive(false);
            return;
        }

        bright.gameObject.SetActive(true);
        bright.color = color;
    }

    internal void SetBarAmount(float amount)
    {
        var radial = (1 - amount) * 360;
        fillBar.material.SetFloat("_Arc2", radial);
    }

    internal void SetBarColor(Color Color)
    {
        fillBar.color = Color;
    }

    public void DisplayMiscText(string message, Color color)
    {
        misc_text.text = message;
        misc_text.color = color;
        misc_shadow_text.text = message;

        misc_text_animator.SetTrigger("Display Text");
    }
}

