using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Factory_Node : NodeView
{
    protected override void OnWorkFinish()
    {
        internalSpeed = 0;

        foreach (var output in outputs)
        {
            output?.SendIngredient();
        }
    }

    protected override void ConnectionMade()
    {
        selectedRecipe = null;

        var ingredients = GetInputIngredients(inputs);

        foreach (var recipe in recipes)
        {
            if (recipe.HasInputIngredients(ingredients))
            {
                selectedRecipe = recipe;
                break;
            }
        }
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
            input.isReadyToClaim = false;
        }

        if (selectedRecipe != null)
            internalSpeed = 1;
    }
}