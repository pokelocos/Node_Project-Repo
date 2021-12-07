using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

    private IngredientView lastView;

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

        //foreach(var recipe in node.GetRecipeInformationStatus())
        //{
        //    GameObject recipeClone = Instantiate(recipePrefab,recipeContainer);

        //    foreach(var ingredient in recipe.inputsStatus)
        //    {
        //        GameObject clone = Instantiate(ingredientPrefab,recipeClone.transform);
        //        clone.transform.SetAsFirstSibling();

        //        if (ingredient.status)
        //        {
        //            Color darker = Color.Lerp(ingredient.ingredient.color, Color.black, 0.5f);
        //            Color bright = Color.Lerp(ingredient.ingredient.color, Color.white, 0.5f);

        //            clone.GetComponent<Image>().color = darker;
        //            clone.transform.GetChild(0).GetComponent<Image>().color = bright;
        //        }

        //        clone.GetComponent<IngredientView>().SetIngredientName(ingredient.ingredient.ingredientName);
        //        clone.GetComponent<IngredientView>().SetIngredientSprite(ingredient.ingredient.icon);
        //    }

        //    if(recipe.originalRecipe.GetInputs().Length > 0 && recipe.originalRecipe.GetOutputs().Length > 0)
        //    {
        //        GameObject imageClone = Instantiate(ingredientPrefab, recipeClone.transform);
        //        imageClone.GetComponentInChildren<Image>().enabled = false;
        //        imageClone.transform.Rotate(new Vector3(0, 0, 90));
        //        imageClone.GetComponent<IngredientView>().SetIngredientSprite(equalSign);
        //        imageClone.GetComponent<IngredientView>().SetActivatable(false);
        //    }

        //    foreach (IngredientData ingredient in recipe.originalRecipe.GetOutputs())
        //    {
        //        GameObject clone = Instantiate(ingredientPrefab,recipeClone.transform);
        //        clone.transform.SetAsLastSibling();
        //        clone.GetComponent<IngredientView>().SetIngredientName(ingredient.ingredientName);
        //        clone.GetComponent<IngredientView>().SetIngredientSprite(ingredient.icon);

        //        if (recipe.canCraft)
        //        {
        //            Color darker = Color.Lerp(ingredient.color, Color.black, 0.5f);
        //            Color bright = Color.Lerp(ingredient.color, Color.white, 0.5f);

        //            clone.GetComponent<Image>().color = darker;
        //            clone.transform.GetChild(0).GetComponent<Image>().color = bright;

        //        }
        //    }

        //    recipes.Add(recipeClone);
        //}
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
