using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeInformationView : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Color innactiveIngredientBackground_color;
    [SerializeField] private Color innactiveIngredientIcon_color;
    [SerializeField] private Color innactiveRecipeResultBackground_color;
    [SerializeField] private Color activeRecipeResultBackground_color;

    [Header("References")]
    [SerializeField] private Image resultsBackground;

    [Header("Components")]
    [SerializeField] private IngredientSlot[] ingredientsSlots;
    [SerializeField] private IngredientView[] resultSlots;

    [System.Serializable]
    public struct IngredientSlot
    {
        public IngredientView ingredientView;
        public GameObject empty;

        public void Show(IngredientData data, Color ingredientBackground_color, Color ingredientIcon_color)
        {
            ingredientView.parent.SetActive(true);

            if (data == null)
            {
                ingredientView.ingredientBackground_sprite.gameObject.SetActive(false);
                empty.SetActive(true);
            }
            else
            {
                ingredientView.Show(data.icon, ingredientBackground_color, ingredientIcon_color);
            }
        }

        public void Hide()
        {
            ingredientView.Hide();
            ingredientView.ingredientBackground_sprite.gameObject.SetActive(false);
            empty.SetActive(false);
        }
    }

    [System.Serializable]
    public struct IngredientView
    {
        public Image ingredientBackground_sprite;
        public Image ingredient_sprite;
        public GameObject parent;

        public void Show(Sprite icon, Color ingredientBackground_color, Color ingredientIcon_color)
        {
            ingredientBackground_sprite.color = ingredientBackground_color;
            ingredient_sprite.color = ingredientIcon_color;

            ingredient_sprite.sprite = icon;

            ingredientBackground_sprite.gameObject.SetActive(true);

            parent.SetActive(true);
        }

        public void Hide()
        {
            parent.SetActive(false);
        }
    }

    public void SetData(RecipeInfo recipeInfo)
    {
        int ingredientAmount = recipeInfo.ingredients.Length;
        int resultAmount = recipeInfo.results.Length;

        if (ingredientAmount + resultAmount > 6)
        {
            Debug.Log("Max ingredients reached. The limit must be 6 ingredients in total.");
            return;
        }

        if (ingredientAmount > ingredientsSlots.Length || resultAmount > resultSlots.Length)
        {
            Debug.Log("Max ingredients reached. The limit must be 5 for both sides.");
            return;
        }

        Clear();

        resultsBackground.color = recipeInfo.validRecipe ? activeRecipeResultBackground_color : innactiveRecipeResultBackground_color;

        for (int i = 0; i < 6 - resultAmount; i++)
        {
            if (i < ingredientAmount)
            {
                if (recipeInfo.ingredients[i].hasColor)
                {
                    ingredientsSlots[i].Show(recipeInfo.ingredients[i].data, recipeInfo.ingredients[i].data.color.Darker(), recipeInfo.ingredients[i].data.color);
                }
                else
                {
                    ingredientsSlots[i].Show(recipeInfo.ingredients[i].data, innactiveIngredientBackground_color, innactiveIngredientIcon_color);
                }
            }
            else
            {
                ingredientsSlots[i].Show(null, innactiveIngredientBackground_color, innactiveIngredientIcon_color);
            }
        }

        for (int i = 0; i < resultSlots.Length; i++)
        {
            if (i < resultAmount)
            {
                if (recipeInfo.results[i].hasColor)
                {
                    resultSlots[i].Show(recipeInfo.results[i].data.icon, recipeInfo.results[i].data.color.Darker(), recipeInfo.results[i].data.color);
                }
                else
                {
                    resultSlots[i].Show(recipeInfo.results[i].data.icon, innactiveIngredientBackground_color, innactiveIngredientIcon_color);
                }

                
            }
        }
    }

    private void Clear()
    {
        foreach (var slot in ingredientsSlots)
        {
            slot.Hide();
        }

        foreach (var slot in resultSlots)
        {
            slot.Hide();
        }
    }
}

[System.Serializable]
public class RecipeInfo
{
    public IngredientInformation[] ingredients;
    public IngredientInformation[] results;

    public bool validRecipe;

    public RecipeInfo(NodeController node, Recipe recipe)
    {
        List<IngredientInformation> validIngredients = new List<IngredientInformation>();
        List<IngredientInformation> validResults = new List<IngredientInformation>();

        List<Port> usedPorts = new List<Port>();

        foreach (var ingredient in recipe.GetIngredients())
        {
            bool isFound = false;

            foreach (var inputPort in node.GetInputPorts())
            {
                if (recipe.CanBeUsedIn(inputPort.Product.data) && !usedPorts.Contains(inputPort))
                {
                    isFound = inputPort.connection != null;

                    if (isFound)
                    {
                        usedPorts.Add(inputPort);
                    }

                    break;
                }
            }

            validIngredients.Add(new IngredientInformation(ingredient.IngredientData, isFound));
        }

        validRecipe = node.GetValidRecipes().Contains(recipe);

        foreach (var result in recipe.GetResults())
        {
            validResults.Add(new IngredientInformation(result, validRecipe));
        }

        ingredients = validIngredients.ToArray();
        results = validResults.ToArray();
    }

    [System.Serializable]
    public struct IngredientInformation
    {
        public IngredientData data;
        public bool hasColor;

        public IngredientInformation(IngredientData data, bool hasColor)
        {
            this.data = data;
            this.hasColor = hasColor;
        }
    }
}