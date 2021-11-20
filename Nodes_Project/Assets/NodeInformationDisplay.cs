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
                clone.GetComponentsInChildren<Image>()[clone.GetComponentsInChildren<Image>().Length-1].sprite = ingredient.icon;
            }
            recipes.Add(recipeClone);
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
