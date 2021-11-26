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

        int i = 0;

        foreach (var input in inputs)
        {
            if (input != null && input.GetIngredient() != null)
            {
                var ing = Instantiate(input.GetIngredient());

                if (i > 0)
                    ing.ingredientName = "Any " + i;
                else
                    ing.ingredientName = "Any";

                ingredients.Add(ing);
                i++;
            }
        }

        var recipe = Instantiate(selectedRecipe);
        recipe.SetInputs(ingredients.ToArray());


        if (ingredients.Count != 3)
        {
            recipe.SetOutputs(new Ingredient[0]);
            return new Recipe[1] { recipe };
        }

        return new Recipe[1] { recipe };
    }

    public override RecipeInformationData[] GetRecipeInformationStatus()
    {
        var recipesStatusData = new List<RecipeInformationData>();

        foreach (var recipe in GetRecipes())
        {
            var data = new RecipeInformationData(recipe);

            foreach (var input in GetInputs())
            {
                if (input != null && input.GetIngredient() != null)
                {
                    if (input.GetIngredient().ingredientName != "Canned Food")
                    {
                        for (int i = 0; i < data.inputsStatus.Count; i++)
                        {
                            if (data.inputsStatus[i].status == false)
                            {
                                data.inputsStatus[i] = new RecipeInformationData.IngredientStatus(input.GetIngredient(), true);
                                break;
                            }
                        }
                    }
                }
            }

            bool canCraft = true;

            foreach (var ingredient in data.inputsStatus)
            {
                if (ingredient.status == false)
                {
                    canCraft = false;
                    break;
                }
            }

            data.canCraft = canCraft;

            recipesStatusData.Add(data);
        }

        return recipesStatusData.ToArray();
    }
}