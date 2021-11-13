using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeInformationDisplay : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Image color;
    [SerializeField] private Text nodeName;
    [SerializeField] private Text description;

    [SerializeField] private RectTransform recipeContainer;
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private GameObject recipePrefab;

    public void SetData(NodeView view)
    {
        color.color = view.GetNodeColor();
        icon.sprite = view.GetIcon();
        nodeName.text = view.GetNodeName();
        description.text = view.GetNodeDescription();
    }

    public void SetRecipes(NodeView view)
    {
        Debug.Log(view.GetNodeData().recipes.Length);

        foreach(Recipe recipe in view.GetNodeData().recipes)
        {
            GameObject recipeClone = Instantiate(recipePrefab,recipeContainer);
            foreach(Ingredient ingredient in recipe.GetInputs())
            {
                GameObject clone = Instantiate(ingredientPrefab,recipeClone.transform);
                clone.transform.SetAsFirstSibling();
                clone.GetComponentInChildren<Image>().sprite = ingredient.icon;
            }
            foreach (Ingredient ingredient in recipe.GetOutputs())
            {
                GameObject clone = Instantiate(ingredientPrefab,recipeClone.transform);
                clone.transform.SetAsLastSibling();
                clone.GetComponentInChildren<Image>().sprite = ingredient.icon;
            }
        }
    }

    public void DestroyPanel()
    {
        Destroy(this.gameObject);
    }
}
