using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IngredientView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image ingredientSprite;
    [SerializeField] private GameObject nameContainer;
    [SerializeField] private Text ingredientName;
    [SerializeField] private bool isActivatable = true;

    public void SetIngredientName(string name)
    {
        ingredientName.text = name;
    }

    public void SetIngredientSprite(Sprite ingredient)
    {
        ingredientSprite.sprite = ingredient;
    }

    public void SetActiveNameContainer(bool active)
    {
        if(isActivatable)
            nameContainer.SetActive(active);
    }

    public void SetActivatable(bool b)
    {
        isActivatable = b;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetActiveNameContainer(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetActiveNameContainer(false);
    }
}
