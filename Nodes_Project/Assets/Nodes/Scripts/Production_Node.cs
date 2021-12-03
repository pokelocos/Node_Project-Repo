using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Production_Node : NodeView
{
    private void Start()
    {
        internalSpeed = 1;

        //The production Nodes should have only one recipe with only outputs.
        selectedRecipe = GetRecipes()[0];
    }
    protected override void OnWorkFinish()
    {
        bool success = false;

        for (int i = 0; i < outputs.Length; i++)
        {
            if(outputs[i] != null)
            {
                success = true;
                break;
            }

        }

        foreach (var output in outputs)
        {
            output?.SendIngredient(output?.GetIngredient());
        }

        if (success)
            GetComponent<Animator>().SetTrigger("Success");
        else
            GetComponent<Animator>().SetTrigger("Fail");
    }

    public override int CanConnectWith(NodeView inputNode)
    {
        return 0;

        if (inputNode.GetInputs().Length == 0)
        {
            return 0;
        }

        for (int i = 0; i < outputs.Length; i++)
        {
            if (outputs[i] == null)
            {
                if (inputNode.GetNodeName() == "Supermarket" || inputNode.GetNodeName() == "Packaging")
                {
                    int occupiedInputs = 0;

                    for (int j = 0; j < inputNode.GetInputs().Length; j++)
                    {
                        if (inputNode.GetInputs()[j] != null)
                            occupiedInputs++;
                    }

                    if (occupiedInputs == inputNode.GetInputs().Length)
                        return 1;
                    else
                    {
                        foreach (var input in inputNode.GetInputs())
                        {
                            if (input != null)
                            {
                                if (input.GetOrigin() == this)
                                {
                                    return 1;
                                }
                            }
                        }

                        return 2;
                    }
                }

                int ingredientMatches = 0;

                var outputIngredient = GetCurrentRecipe().GetOutputs()[i];

                foreach (var recipe in inputNode.GetRecipes())
                {
                    if (recipe.GetInputs().Contains(outputIngredient))
                    {
                        ingredientMatches++;
                    }
                }

                if (ingredientMatches > 0)
                {
                    var currentIngredients = new List<Ingredient>();

                    foreach (var input in inputNode.GetInputs())
                    {
                        if (input != null)
                        {
                            currentIngredients.Add(input.GetIngredient());
                        }
                    }

                    if (currentIngredients.Count < inputNode.GetInputs().Length && !currentIngredients.Contains(outputIngredient))
                    {
                        ingredientMatches = 0;

                        currentIngredients.Add(outputIngredient);

                        foreach (var recipe in inputNode.GetRecipes())
                        {
                            ingredientMatches = recipe.InputIngredientsMatchCount(currentIngredients.ToArray());

                            if (ingredientMatches == currentIngredients.Count)
                            {
                                foreach (var input in inputNode.GetInputs())
                                {
                                    if (input != null)
                                    {
                                        if (input.GetOrigin() == this)
                                        {
                                            return 1;
                                        }
                                    }
                                }

                                return 2;
                            }
                        }
                    }

                    return 1;
                }

                return 0;
            }
        }

        return 0;
    }

    public override void ConnectionChange()
    {
       
    }

    public override void InputIngredientReady(ConnectionView connection)
    {
       
    }

    public override RecipeInformationData[] GetRecipeInformationStatus()
    {
        var recipesStatusData = new List<RecipeInformationData>();

        foreach (var recipe in GetRecipes())
        {
            var data = new RecipeInformationData(recipe);

            for (int i = 0; i < data.inputsStatus.Count; i++)
            {
                data.inputsStatus[i] = new RecipeInformationData.IngredientStatus(data.inputsStatus[i].ingredient, true);
            }

            data.canCraft = true;

            recipesStatusData.Add(data);
        }

        return recipesStatusData.ToArray();
    }

    public override Recipe[] ValidRecipes()
    {
        return new Recipe[1] { GetCurrentRecipe() };
    }
}
