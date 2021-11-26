using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seller_Node : NodeView
{
    Ingredient[] ingredients;

    [SerializeField] private Animator price_animation;
    [SerializeField] private TextMesh price;
    [SerializeField] private TextMesh price_shadow;

    protected override void OnWorkFinish()
    {
        internalSpeed = 0;

        var money = 0;
        foreach (var input in inputs)
        {
            if (input != null)
            {
                money += input.GetOutputIngredient().price;
            }
        }

        money *= 2;

        bool success = false;

        for (int i = 0; i < inputs.Length; i++)
        {
            if (inputs[i] != null)
            {
                success = true;
                break;
            }

        }

        if (success)
        {
            GetComponent<Animator>().SetTrigger("Success");

            DisplayMoney(money);
        }
        else
            GetComponent<Animator>().SetTrigger("Fail");

        GameManager.AddMoney(money);
    }

    protected void DisplayMoney(float money)
    {
        price.text = "$" + money;
        price_shadow.text = "$" + money;
        price_animation.SetTrigger("Show Money");
    }

    public override void ConnectionChange()
    {
        selectedRecipe = null;

        ingredients = GetInputIngredients(inputs);

        foreach (var recipe in GetRecipes())
        {
            if (recipe.HasInputIngredients(ingredients))
            {
                selectedRecipe = recipe;
                break;
            }
        }
    }

    public override int CanConnectWith(NodeView inputNode)
    {
        return 0;
    }

    public override void InputIngredientReady(ConectionView connection)
    {
        foreach (var input in inputs)
        {
            if (input != null)
            {
                if (!input.isReadyToClaim)
                {
                    return;
                }
            }
        }

        foreach (var input in inputs)
        {
            if (input)
            {
                input.isReadyToClaim = false;
            }
        }

        if (selectedRecipe != null)
            internalSpeed = 1;
    }

    public override Recipe[] ValidRecipes()
    {
        return new Recipe[1] { selectedRecipe };
    }
}
