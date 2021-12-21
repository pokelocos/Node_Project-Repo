using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeInformationView : MonoBehaviour
{
    private bool isDragging;
    private Vector3 dragPoint;
    private int currentPageIndex;

    [Header("Components")]
    [SerializeField] private RectTransform[] pageButtons;
    [SerializeField] private GameObject[] pages;

    [Header("References / Recipe Page")]
    [SerializeField] private RecipeInformationView recipeInformationView_template;
    [SerializeField] private Transform recipes_content;

    [Header("References / Reume")]
    [SerializeField] private Text nodeName_text;
    [SerializeField] private Text nodeDesc_text;
    [SerializeField] private Text stats_productionTime_text;
    [SerializeField] private Text stats_successRate_text;
    [SerializeField] private Text stats_maintainCosts_text;
    [SerializeField] private Image icon_image;
    [SerializeField] private Image iconBackground_image;

    private NodeController currentNode;

    public void DisplayInformation(NodeController node)
    {
        currentNode = node;

        gameObject.SetActive(true);
        transform.GetChild(0).localPosition = Vector3.zero;

        DisplayResume(node);

        ChangePage(0);
    }

    private void Update()
    {
        ManagePageButtons();
    }

    public void BeginDrag()
    {
        isDragging = true;

        dragPoint = transform.GetChild(0).position - Input.mousePosition;
    }

    private void DisplayResume(NodeController node)
    {
        nodeName_text.text = node.NodeView.GetNodeName();
        nodeDesc_text.text = node.NodeView.GetNodeDescription();
        stats_productionTime_text.text = node.NodeView.GetNodeData().productionTime + "s";
        stats_successRate_text.text = "%" +node.NodeView.GetNodeData().successProbability * 100;
        stats_maintainCosts_text.text = "$" +node.NodeView.GetNodeData().maintainCost;

        icon_image.sprite = node.NodeView.GetIcon();
        iconBackground_image.sprite = node.NodeView.GetIcon();
    }

    private void ManagePageButtons()
    {
        for (int i = 0; i < pageButtons.Length; i++)
        {
            if (i == currentPageIndex)
            {
                pageButtons[i].sizeDelta = new Vector2(pageButtons[i].sizeDelta.x, Mathf.Lerp(pageButtons[i].sizeDelta.y, 70, Time.unscaledDeltaTime * 5));
            }
            else
            {
                pageButtons[i].sizeDelta = new Vector2(pageButtons[i].sizeDelta.x, Mathf.Lerp(pageButtons[i].sizeDelta.y, 50, Time.unscaledDeltaTime * 5));
            }
        }
    }

    public void ChangePage(int index)
    {
        pages[currentPageIndex].SetActive(false);
        currentPageIndex = index;
        pages[currentPageIndex].SetActive(true);

        switch (index)
        {
            case 0:
                DisplayProductionPage();
                break;
            case 1:
                DisplayConnectionsPage();
                break;
            case 2:
                DisplayStatusPage();
                break;
        }
    }

    private void DisplayProductionPage()
    {
        foreach (Transform child in recipes_content.transform)
        {
            if (child.gameObject != recipeInformationView_template.gameObject)
            {
                Destroy(child.gameObject);
            }
        }

        recipeInformationView_template.gameObject.SetActive(false);

        if (currentNode == null)
            return;

        var usedRecipes = new List<Recipe>();
        var usedIngredients = new List<IngredientData>();

        foreach (var recipe in currentNode.GetValidRecipes())
        {
            if (!usedRecipes.Contains(recipe.Key))
            {
                usedRecipes.Add(recipe.Key);

                var slot = Instantiate(recipeInformationView_template, recipes_content);
                slot.gameObject.SetActive(true);

                slot.SetData(new RecipeInfo(RecipeInfo.ConvertFrom(recipe.Value.Select(x => x.Product.data).ToArray(), true), RecipeInfo.ConvertFrom(recipe.Key.GetResults(), true), true)); 
            }
        }
    }

    private void DisplayConnectionsPage()
    {

    }

    private void DisplayStatusPage()
    {

    }

    public void Drag()
    {
        transform.GetChild(0).position = dragPoint + Input.mousePosition;
    }

    public void StopDrag()
    {
        isDragging = true;
    }
}
