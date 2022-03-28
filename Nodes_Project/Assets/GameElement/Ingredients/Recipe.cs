using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu]
public class Recipe : ScriptableObject
{
    [System.Obsolete("This is an obsolete variable")]
    [SerializeField] IngredientData[] inputs;
    [System.Obsolete("This is an obsolete variable")]
    [SerializeField] IngredientData[] outputs;

    [SerializeField] private Ingredient[] ingredients;
    [SerializeField] private IngredientData[] results;

    [System.Obsolete("This is an obsolete method")]
    public void SetInputs(IngredientData[] ingredients)
    {
        inputs = ingredients;
    }

    [System.Obsolete("This is an obsolete method")]
    public void SetOutputs(IngredientData[] ingredients)
    {
        outputs = ingredients;
    }

    [System.Obsolete("This is an obsolete method")]
    public IngredientData[] GetOutputs()
    {
        return outputs;
    }

    [System.Obsolete("This is an obsolete method")]
    public IngredientData[] GetInputs()
    {
        return inputs;
    }

    public Ingredient[] GetIngredients()
    {
        return ingredients;
    }

    public IngredientData[] GetResults()
    {
        return results;
    }

    /// <summary>
    /// Generate products with this recipe.
    /// </summary>
    /// <param name="extraCost"></param>
    /// <returns></returns>
    public Product[] GenerateProducts(Product[] rawMaterials, int extraCost)
    {
        Product[] products = new Product[results.Length];

        for (int i = 0; i < products.Length; i++)
        {
            products[i] = new Product(results[i], rawMaterials.Sum(x => x.currentValue) + extraCost, rawMaterials);
        }

        return products;
    }

    [System.Obsolete("This is an obsolete method")]
    public int OutputIngredientsMatchCount(IngredientData[] ingredients)
    {
        int matches = 0;

        foreach (var ingredient in ingredients)
        {
            if (outputs.Contains(ingredient))
                matches++;
        }

        return matches;
    }

    [System.Obsolete("This is an obsolete method")]
    public bool HasInputIngredients(IngredientData[] listToCheck)
    {
        foreach(IngredientData ingredient in inputs)
        {
            if(!listToCheck.Contains(ingredient))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Check if all ingredients matches in this recipe.
    /// </summary>
    /// <param name="ingredients"></param>
    /// <returns></returns>
    public int InputIngredientsMatchCount(IngredientData[] ingredients)
    {
        int matches = 0;

        foreach (var ingredient in ingredients)
        {
            if (ingredients.Contains(ingredient))
                matches++;
        }

        return matches;
    }


    /// <summary>
    /// Return true if certain ingredientData is valid in a group of ingredients.
    /// </summary>
    /// <param name="ingredientData"></param>
    /// <param name="recipeIngredients"></param>
    /// <returns></returns>
    public static bool CanBeUsedIn(IngredientData ingredientData, Ingredient[] recipeIngredients, out Ingredient matchIngredient)
    {
        matchIngredient = null;
        string log = string.Empty;
        foreach (var ingredient in recipeIngredients)
        {
            log += "Checking " + ingredientData.ingredientName + " with " + ingredient.IngredientData.name + " -Opt " + ingredient.IsOptional;

            if (CanBeUsedIn(ingredientData, ingredient))
            {
                matchIngredient = ingredient;

                log += " = SUCCESS";
                return true;
            }

            log += " = FAIL\n";
        }

       // Debug.Log(log);
        return false;
    }

    /// <summary>
    /// Return true if an ingredientData can be used by ingredient.
    /// </summary>
    /// <param name="ingredientData"></param>
    /// <param name="ingredient"></param>
    /// <returns></returns>
    public static bool CanBeUsedIn(IngredientData ingredientData, Ingredient ingredient)
    {
        bool byIngredient = false;
        bool byTag = false;
        bool specifigTag = false;
        bool itsAny = false;

        if (ingredient.CompareIngredient)
        {
            byIngredient = ingredient.IngredientData.Equals(ingredientData);
        }

        if (ingredient.CompareTags)
        {
            byTag = ingredient.IngredientData.tags.Intersect(ingredientData.tags).Any();
        }

        if (ingredient.CompareSpecificTags)
        {
            specifigTag = ingredientData.tags.Any(x => ingredient.SpecificTags.Contains(x));
        }

        if (ingredient.SpecificTags.Contains("ANY") || ingredient.IngredientData.tags.Contains("ANY"))
        {
            itsAny = true;
        }

        if (byIngredient || byTag || specifigTag || ingredient.IsOptional || itsAny)
        {
            return true;
        }

        return false;
    }
}

[System.Serializable]
public class Ingredient
{
    [SerializeField]
    private IngredientData ingredientData;

    public IngredientData IngredientData
    {
        get
        {
            return ingredientData;
        }
    }

    public bool CompareIngredient
    {
        get
        {
            return compareItem;
        }
    }

    public bool CompareTags
    {
        get
        {
            return compareItemTags;
        }
    }

    public bool CompareSpecificTags
    {
        get
        {
            return compareSpecificTags;
        }
    }

    public bool IsOptional
    {
        get
        {
            return isOptional;
        }
    }

    public string[] SpecificTags
    {
        get
        {
            return specificTags;
        }
    }

    [Header("Settings")]
    [SerializeField]
    private bool compareItem = true;

    [SerializeField]
    private bool compareItemTags;

    [SerializeField]
    private bool compareSpecificTags;

    [SerializeField]
    private string[] specificTags;

    [SerializeField]
    private bool isOptional;

    public static string ListToString(IEnumerable<Ingredient> list)
    {
        string text = string.Empty;

        foreach (var item in list)
        {
            text += item.ingredientData.ingredientName + " + ";
        }

        return text;
    }
}

public static class RecipeExtensions
{
    /// <summary>
    /// Return true if certain ingredientData is valid in a group of ingredients.
    /// </summary>
    /// <param name="recipe"></param>
    /// <param name="ingredientData"></param>
    /// <param name="matchIngredient"></param>
    /// <returns></returns>
    public static bool CanBeUsedIn(this Recipe recipe, IngredientData ingredientData, out Ingredient matchIngredient)
    {
        return Recipe.CanBeUsedIn(ingredientData, recipe.GetIngredients(), out matchIngredient);
    }

    /// <summary>
    /// Return true if certain ingredientData is valid in a group of ingredients.
    /// </summary>
    /// <param name="recipe"></param>
    /// <param name="ingredientData"></param>
    /// <returns></returns>
    public static bool CanBeUsedIn(this Recipe recipe, IngredientData ingredientData)
    {
        Ingredient outAux = null;
        return Recipe.CanBeUsedIn(ingredientData, recipe.GetIngredients(), out outAux);
    }
}
