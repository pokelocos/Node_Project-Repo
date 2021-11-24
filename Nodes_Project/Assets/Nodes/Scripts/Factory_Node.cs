using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Factory_Node : NodeView
{
    protected override void OnWorkFinish()
    {
        internalSpeed = 0;

        int price = 0;

        foreach (var input in inputs)
        {
            if (input != null)
            {
                if (input.GetOutputIngredient() != null)
                {
                    price += input.GetOutputIngredient().price;
                }
            }
        }

        price *= 2;

        foreach (var output in outputs)
        {
            if (output != null)
            {
                Ingredient ingredient = Instantiate(output.GetIngredient());

                ingredient.price += price;

                output.SendIngredient(ingredient);
            }
        }

        bool success = false;

        for (int i = 0; i < outputs.Length; i++)
        {
            if (outputs[i] != null)
            {
                success = true;
                break;
            }

        }

        if (success)
            GetComponent<Animator>().SetTrigger("Success");
        else
            GetComponent<Animator>().SetTrigger("Fail");
    }

    public override void ConnectionChange()
    {
        selectedRecipe = null;

        var ingredients = GetInputIngredients(inputs);

        foreach (var recipe in GetRecipes())
        {
            if (recipe.HasInputIngredients(ingredients))
            {
                selectedRecipe = recipe;
                break;
            }
        }

        if (selectedRecipe == null)
        {
            foreach (var output in outputs)
            {
                if (output != null)
                    output.Disconnect();
            }
        }
    }

    public override int CanConnectWith(NodeView inputNode)
    {
        if (GetCurrentRecipe() != null)
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
                }
            }

            return 0;
        }

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
           if(input != null)
                input.isReadyToClaim = false;
        }

        if (selectedRecipe != null)
            internalSpeed = 1;
    }
    public override Recipe[] ValidRecipes()
    {
        return new Recipe[1] { GetCurrentRecipe() };
    }
}