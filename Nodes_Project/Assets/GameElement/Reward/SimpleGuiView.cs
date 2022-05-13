using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleGuiView : MonoBehaviour // claramente no es el mejor nombre pero es lo que se ocurrio para un generico con icono
{
    [SerializeField] private Image icon;
    [SerializeField] private Image background;
    [SerializeField] private GameObject newPanel;

    // events
    public delegate void SimpleGuiEvent(SimpleGuiView simpleGuiView);
    public SimpleGuiEvent onMouseEnter;
    public SimpleGuiEvent onMouseExit;

    public void SetInfo(Sprite sprite, Color color, bool isNew = false)
    {
        this.icon.sprite = sprite;
        this.background.color = color;

        newPanel.SetActive(isNew);
    }

    private void OnMouseEnter()
    {
        onMouseEnter?.Invoke(this);
        newPanel.SetActive(false);
    }

    private void OnMouseExit()
    {
        onMouseExit?.Invoke(this);
        
    }
}
