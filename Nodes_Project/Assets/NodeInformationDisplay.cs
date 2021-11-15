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

    [SerializeField] private RectTransform recipeContainer;
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private GameObject recipePrefab;

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
        foreach(Recipe recipe in node.GetNodeData().recipes)
        {
            GameObject recipeClone = Instantiate(recipePrefab,recipeContainer);
            foreach(Ingredient ingredient in recipe.GetInputs())
            {
                GameObject clone = Instantiate(ingredientPrefab,recipeClone.transform);
                clone.transform.SetAsFirstSibling();
                clone.GetComponentsInChildren<Image>()[clone.GetComponentsInChildren<Image>().Length - 1].sprite = ingredient.icon;
            }
            foreach (Ingredient ingredient in recipe.GetOutputs())
            {
                GameObject clone = Instantiate(ingredientPrefab,recipeClone.transform);
                clone.transform.SetAsLastSibling();
                clone.GetComponentsInChildren<Image>()[clone.GetComponentsInChildren<Image>().Length-1].sprite = ingredient.icon;
            }
        }
    }

    public void DestroyPanel()
    {
        Destroy(this.gameObject);
    }
}
