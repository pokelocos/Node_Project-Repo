using RA.InputManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static NodeManager;

public class IngredientProxyView : MonoBehaviour
{
    [Header("Varibles")]
    [SerializeField] private Vector3 offset = new Vector3(0.5f, 1, 0);
    [SerializeField] private Sprite invalidIngredient;

    [Header("Pref references")]
    [SerializeField] private Image icon;
    [SerializeField] private Image background;
    [SerializeField] private Image border;
    [SerializeField] private Text price;
    [SerializeField] private Text priceShadow;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        this.transform.position = Input.mousePosition + offset;
    }

    public void ShowInformation(Filters filters, Product product)
    {
        this.gameObject.SetActive(true);
        if (product != null)
        {
            icon.sprite = product.data.icon;
            SetColors(product.data.primaryColor, product.data.secondaryColor);
            SetText("$" + product.currentValue);
        }
        else
        {
            icon.sprite = invalidIngredient;
            SetText("");
        }
    }

    public void HidenInformation()
    {
        this.gameObject.SetActive(false);
    }

    private void SetColors(Color c1, Color c2)
    {
        background.color = c1;
        border.color = c2;
        icon.color = c2;
    }

    private void SetText (string text)
    {
        price.text = text;
        priceShadow.text = text;
    }
}
