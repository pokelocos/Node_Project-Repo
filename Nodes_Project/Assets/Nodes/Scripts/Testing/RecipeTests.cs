using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeTests : MonoBehaviour
{
    [SerializeField] private Recipe recipe;

    [Space]

    [SerializeField] private Dropdown ingredientSelector;
    [SerializeField] private GameObject ingredientTemplate_slot;
    [SerializeField] private Transform ingredients_content;
    [SerializeField] private Text recipeData_text;
    [SerializeField] private GameObject usedIngredientSlot_slot;
    [SerializeField] private Transform usedIngredients_content;

    private IngredientData[] allIngredients;

    public List<IngredientData> inputs = new List<IngredientData>();
    private Dictionary<Recipe, List<IngredientData>> selectedRecipes = new Dictionary<Recipe, List<IngredientData>>();

    private void Start()
    {
        allIngredients = Resources.LoadAll<IngredientData>("Datas/Ingredients");
        ingredientSelector.AddOptions(allIngredients.Select(x => new Dropdown.OptionData(x.ingredientName)).ToList());

        ingredientTemplate_slot.SetActive(false);
        usedIngredientSlot_slot.SetActive(false);
    }

    private void Update()
    {
        recipeData_text.text = recipe != null? "Used Ingredients By:\n" + recipe.name : "Used Ingredients By:\nSelect Recipe";

        if (inputs.Count > 0)
        {
            for (int i = 1; i < ingredients_content.childCount; i++)
            {
                ingredients_content.GetChild(i).GetComponentInChildren<Text>().text = inputs[i - 1].ingredientName;
            }
        }
    }

    public void AddIngredient(int i)
    {
        if (i == 0)
            return;

        inputs.Add(allIngredients[i - 1]);

        var slot = Instantiate(ingredientTemplate_slot, ingredients_content);
        slot.SetActive(true);
    }

    public void RemoveIngredient(Transform slot)
    {
        inputs.RemoveAt(slot.GetSiblingIndex() - 1);

        Destroy(slot.gameObject);
    }

    public void CheckRecipes()
    {
        selectedRecipes.Clear();

        //Calculate all possible combinations
        var inputIngredientPorts = inputs;

        List<List<IngredientData>> allCombinations = new List<List<IngredientData>>();

        for (int i = 1; i <= inputIngredientPorts.Count; i++)
        {
            List<List<IngredientData>> combinations = Combinations<IngredientData>.GetCombinations(inputIngredientPorts, i);

            allCombinations.AddRange(combinations);
        }

        //Generate the list of possible Recipes
        var validRecipes = new Dictionary<Recipe, List<IngredientData>>();

        foreach (var combination in allCombinations.OrderByDescending(x => x.Count))
        {
            bool validRecipe = true;
            var ingredients = recipe.GetIngredients().OrderByDescending(x => !x.IsOptional).ToList();
            var validPorts = new List<IngredientData>();

            print(ListToString(combination));

            foreach (var candidate in combination)
            {
                if (ingredients.Count > 0)
                {
                    Ingredient matchIngredient = null;

                    if (Recipe.CanBeUsedIn(candidate, ingredients.ToArray(), out matchIngredient))
                    {
                        ingredients.Remove(matchIngredient);
                        validPorts.Add(candidate);
                    }
                    else
                    {
                        validRecipe = false;
                        break;
                    }
                }
                else
                {
                    validRecipe = false;
                    break;
                }
            }

            //Check if unnused items are optionals
            foreach (var ingredient in ingredients)
            {
                if (!ingredient.IsOptional)
                    validRecipe = false;
            }

            if (validRecipe && !validRecipes.ContainsKey(recipe))
            {
                validRecipes.Add(recipe, validPorts);
                break;
            }
        }

        //Select the best recipes
        validRecipes = validRecipes.OrderByDescending(x => x.Key.GetIngredients().Length).ToDictionary(x => x.Key, y => y.Value); //Order by recipe input size
        selectedRecipes = validRecipes;

        foreach (Transform child in usedIngredients_content)
        {
            if (child.gameObject != usedIngredientSlot_slot)
            {
                Destroy(child.gameObject);
            }
        }

        if (selectedRecipes.Count > 0)
        {
            foreach (var ingredients in selectedRecipes.Values)
            {
                foreach (var ingredient in ingredients)
                {
                    var slot = Instantiate(usedIngredientSlot_slot, usedIngredients_content);
                    slot.SetActive(true);

                    for (int i = 0; i < inputs.Count; i++)
                    {
                        if (inputs[i] == ingredient)
                        {
                            slot.GetComponentInChildren<Text>().text = ingredient.ingredientName + "(" + i + ")";
                            break;
                        }
                    }
                }
            }
        }
    }

    public static string ListToString(IEnumerable list)
    {
        string log = "";

        foreach (var item in list)
        {
            log += item.ToString() + " +";
        }

        log = log.Substring(0, log.Length - 1);

        return log;
    }
}

