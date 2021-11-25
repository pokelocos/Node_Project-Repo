using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeInformationDisplay : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image color;
    [SerializeField] private Text nameText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private Text speedText;
    [SerializeField] private Sprite equalSign;

    [SerializeField] private RectTransform recipeContainer;
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private GameObject recipePrefab;

    private List<GameObject> recipes = new List<GameObject>();

    public void SetData(NodeData node)
    {
        color.color = node.color;
        icon.sprite = node.icon;
        nameText.text = node.name;
        //descriptionText.text = node.description;
        speedText.text = node.speed + " s.";
    }

    public void SetRecipes(NodeView node) // NodeData
    {
        ClearRecipeList();
        foreach(Recipe recipe in node.GetNodeData().recipes)
        {
            GameObject recipeClone = Instantiate(recipePrefab,recipeContainer);

            foreach(Ingredient ingredient in recipe.GetInputs())
            {
                GameObject clone = Instantiate(ingredientPrefab,recipeClone.transform);
                clone.transform.SetAsFirstSibling();

                foreach (var input in node.GetInputs())
                {
                    if (input != null && input.GetIngredient() != null)
                    {
                        if (input.GetIngredient() == ingredient)
                        {
                            Color darker = Color.Lerp(input.GetIngredient().color, Color.black, 0.5f);
                            Color bright = Color.Lerp(input.GetIngredient().color, Color.white, 0.5f);

                            clone.GetComponent<Image>().color = darker;
                            clone.transform.GetChild(0).GetComponent<Image>().color = bright;
                        }
                    }
                }

                clone.name = ingredient.ingredientName;
                clone.GetComponentsInChildren<Image>()[clone.GetComponentsInChildren<Image>().Length - 1].sprite = ingredient.icon;
            }

            if(recipe.GetInputs().Length > 0 && recipe.GetOutputs().Length > 0)
            {
                GameObject imageClone = Instantiate(ingredientPrefab, recipeClone.transform);
                imageClone.GetComponentInChildren<Image>().enabled = false;
                imageClone.transform.Rotate(new Vector3(0, 0, 90));
                imageClone.GetComponentsInChildren<Image>()[imageClone.GetComponentsInChildren<Image>().Length - 1].sprite = equalSign;
            }

            foreach (Ingredient ingredient in recipe.GetOutputs())
            {
                GameObject clone = Instantiate(ingredientPrefab,recipeClone.transform);
                clone.transform.SetAsLastSibling();
                clone.name = ingredient.ingredientName;
                clone.GetComponentsInChildren<Image>()[clone.GetComponentsInChildren<Image>().Length-1].sprite = ingredient.icon;
            }

            recipes.Add(recipeClone);

            int anyMatches = 0;

            for (int i = 0; i < recipeClone.transform.childCount; i++)
            {
                if(recipeClone.transform.GetChild(i).name == "Any")
                {
                    if (anyMatches > 0)
                    {
                        recipeClone.transform.GetChild(i).name = "Any " + anyMatches;
                    }

                    anyMatches++;
                }
            }
        }

        var validRecipes = node.ValidRecipes();

        foreach (var recipe in validRecipes)
        {
            if (recipe != null)
            {
                foreach (var ingredientInput in recipe.GetInputs())
                {
                    foreach (Transform recipeTransform in recipeContainer)
                    {
                        foreach (Transform ingredientTransform in recipeTransform)
                        {
                            if (ingredientTransform.name == ingredientInput.ingredientName)
                            {
                                Color darker = Color.Lerp(ingredientInput.color, Color.black, 0.5f);
                                Color bright = Color.Lerp(ingredientInput.color, Color.white, 0.5f);

                                ingredientTransform.GetComponent<Image>().color = darker;
                                ingredientTransform.GetChild(0).GetComponent<Image>().color = bright;
                            }
                        }
                    }
                }

                foreach (var ingredientOutput in recipe.GetOutputs())
                {
                    foreach (Transform recipeTransform in recipeContainer)
                    {
                        foreach (Transform ingredientTransform in recipeTransform)
                        {
                            if (ingredientTransform.name == ingredientOutput.ingredientName)
                            {
                                Color darker = Color.Lerp(ingredientOutput.color, Color.black, 0.5f);
                                Color bright = Color.Lerp(ingredientOutput.color, Color.white, 0.5f);

                                ingredientTransform.GetComponent<Image>().color = darker;
                                ingredientTransform.GetChild(0).GetComponent<Image>().color = bright;
                            }
                        }
                    }
                }
            }
        }

        
    }

    private void ClearRecipeList()
    {
        foreach(GameObject g in recipes)
        {
            Destroy(g);
        }
    }

    public void DestroyPanel()
    {
        Destroy(this.gameObject);
    }
}
