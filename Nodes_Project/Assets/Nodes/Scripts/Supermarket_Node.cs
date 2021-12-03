using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Supermarket_Node : Seller_Node
{
    private void Start()
    {
        inputs = new ConnectionView[4];
    }

    protected override void OnWorkFinish()
    {
        internalSpeed = 0;

        var money = 0;

        foreach (var input in inputs)
        {
            if (input != null)
            {
                var ingredient = input.GetOutputIngredient();

                if (ingredient != null)
                {
                    if (ingredient.name != "Canned Food")
                    {
                        money += Mathf.CeilToInt(ingredient.price * 0.3f);
                    }
                    else
                    {
                        money += ingredient.price;
                    }
                }

                input.isReadyToClaim = false;
            }
        }

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

    public override int CanConnectWith(NodeView inputNode)
    {
        return 0;
    }

    public override void ConnectionChange()
    {

    }

    public override void InputIngredientReady(ConnectionView connection)
    {
        var readyforSell = inputs.Count(x => x != null && x.isReadyToClaim);

        foreach (var input in inputs)
        {
            if (input != null)
                input.isReadyToClaim = false;
        }


        if (readyforSell > 0)
            internalSpeed = 1;
    }

    public override Recipe[] ValidRecipes()
    {
        List<Recipe> recipes = new List<Recipe>();

        foreach (var input in inputs)
        {
            if (input != null && input.GetIngredient() != null)
            {
                if (input.GetIngredient().ingredientName != "Canned Food" && !recipes.Contains(GetRecipes()[1]))
                {
                    recipes.Add(GetRecipes()[1]);
                }

                if (input.GetIngredient().ingredientName == "Canned Food" && !recipes.Contains(GetRecipes()[0]))
                {
                    recipes.Add(GetRecipes()[0]);
                }
            }
        }

        return recipes.ToArray();
    }

    public override RecipeInformationData[] GetRecipeInformationStatus()
    {
        var recipesStatusData = new List<RecipeInformationData>();

        for (int i = 0; i < GetRecipes().Length; i++)
        {
            var data = new RecipeInformationData(GetRecipes()[i]);

            foreach (var input in GetInputs())
            {
                if (input != null && input.GetIngredient() != null)
                {
                    if (i == 0 && input.GetIngredient().ingredientName == "Canned Food")
                    {
                        for (int j = 0; j < data.inputsStatus.Count; j++)
                        {
                            if (data.inputsStatus[j].status == false)
                            {
                                data.inputsStatus[j] = new RecipeInformationData.IngredientStatus(input.GetIngredient(), true);
                                break;
                            }
                        }
                    }
                    else if (i != 0 && input.GetIngredient().ingredientName != "Canned Food")
                    {
                        for (int j = 0; j < data.inputsStatus.Count; j++)
                        {
                            if (data.inputsStatus[j].status == false)
                            {
                                data.inputsStatus[j] = new RecipeInformationData.IngredientStatus(input.GetIngredient(), true);
                                break;
                            }
                        }
                    }
                }
            }

            bool canCraft = false;

            foreach (var ingredient in data.inputsStatus)
            {
                if (ingredient.status == true)
                {
                    canCraft = true;
                    break;
                }
            }

            data.canCraft = canCraft;

            recipesStatusData.Add(data);
        }

        return recipesStatusData.ToArray();
    }
}
