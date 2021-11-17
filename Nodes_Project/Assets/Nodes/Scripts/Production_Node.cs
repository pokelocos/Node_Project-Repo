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
        selectedRecipe = recipes[0];
    }
    protected override void OnWorkFinish()
    {
        foreach (var output in outputs)
        {
            output?.SendIngredient();
        }
    }

    public override int CanConnectWith(NodeView inputNode)
    {
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
                        return 2;
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
                                return 2;
                        }
                    }

                    return 1;
                }
            }
        }

        return 0;
    }

    public override void ConnectionChange()
    {
       
    }

    public override void InputIngredientReady(ConectionView connection)
    {
       
    }
}
