using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seller_Node : NodeView
{
    Ingredient[] ingredients;

    protected override void OnWorkFinish()
    {
        internalSpeed = 0;

        var money = 0;
        foreach (var input in inputs)
        {
            money += input.GetOutputIngredient().price;
        }

        GameManager.AddMoney(money);
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
}
