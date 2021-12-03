using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Void_Node : Supermarket_Node
{
    [SerializeField] private TextMesh amount_text;
    [SerializeField] private GameObject star;
    public int amount = 50;
    public bool isCompleted;

    private void Start()
    {
        selectedRecipe = GetRecipes()[0];

        inputs = new ConnectionView[4];

        star.SetActive(false);

        amount_text.GetComponent<MeshRenderer>().sortingLayerName = "Node";
        amount_text.GetComponent<MeshRenderer>().sortingOrder = 200;

        amount_text.text = amount.ToString();

        amount_text.gameObject.SetActive(false);
    }

    private void OnMouseEnter()
    {
        amount_text.gameObject.SetActive(true);
        nodeIcon.gameObject.SetActive(false);
    }

    private void OnMouseExit()
    {
        amount_text.gameObject.SetActive(false);
        nodeIcon.gameObject.SetActive(true);
    }

    public override int CanConnectWith(NodeView inputNode)
    {
        return 0;

        if (isCompleted)
            return 0;

        if (inputNode.GetCurrentRecipe() == null)
            return 0;

        Ingredient ingredient = null;

        for (int i = 0; i < inputNode.GetOutputs().Length; i++)
        {
            if (inputNode.GetOutputs()[i] == null)
            {
                ingredient = inputNode.GetCurrentRecipe().GetOutputs()[i];
                break;
            }
        }

        if (ingredient == null)
            return 0;

        if (ingredient.ingredientName == selectedRecipe.GetInputs()[0].ingredientName)
        {
            bool canConnect = false;

            foreach (var input in GetInputs())
            {
                if(input == null)
                {
                    canConnect = true;
                    break;
                }
            }

            if (canConnect)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }

        return 0;
    }

    protected override void OnWorkFinish()
    {
        internalSpeed = 0;

        int substractAmount = 0;

        foreach (var input in inputs)
        {
            if (input != null)
            {
                substractAmount++;
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

        amount -= substractAmount;
        amount_text.text = amount.ToString();

        if (amount <= 0)
        {
            isCompleted = true;

            data.maintainCost = 0;

            GameManager.points++;

            foreach (var input in inputs)
            {
                if (input != null)
                {
                    input.Disconnect();
                }
            }

            inputs = new ConnectionView[0];

            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }

            star.SetActive(true);

            GetComponent<SpriteRenderer>().color = Color.clear;
        }

        if (success)
            GetComponent<Animator>().SetTrigger("Success");
    }

    public override Recipe[] ValidRecipes()
    {
        return new Recipe[1] { GetCurrentRecipe() };
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
                    for (int i = 0; i < data.inputsStatus.Count; i++)
                    {
                        if (input.GetIngredient().ingredientName == data.inputsStatus[i].ingredient.ingredientName)
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