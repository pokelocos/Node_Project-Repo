using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Packaging_Node : Factory_Node
{
    private void Start()
    {
        inputs = new ConectionView[3];

        selectedRecipe = GetRecipes()[0];
    }
    public override void ConnectionChange()
    {

    }

    public override int CanConnectWith(NodeView inputNode)
    {
        if (inputNode.GetNodeName() == "Packaging")
            return 0;

        return base.CanConnectWith(inputNode);
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
            else
            {
                return;
            }
        }

        foreach (var input in inputs)
        {
           if(input != null)
                input.isReadyToClaim = false;
        }

        if (selectedRecipe != null)
            internalSpeed = 1;
    }

    public override Recipe[] ValidRecipes()
    {
        List<Ingredient> ingredients = new List<Ingredient>();

        foreach (var input in inputs)
        {
            if (input != null && input.GetIngredient() != null)
            {
                var ing = Instantiate(input.GetIngredient());
                ing.ingredientName = "Any";
                ingredients.Add(ing);
            }
        }

        if (ingredients.Count == 3)
        {
            var recipe = Instantiate(selectedRecipe);
            recipe.SetInputs(ingredients.ToArray());

            return new Recipe[1] { recipe };
        }

        return new Recipe[0];
    }
}