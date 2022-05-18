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

    [Header("References / Connections Page")]
    [SerializeField] private ConnectionInformationView connectionInfoView_template;
    [SerializeField] private Transform inputs_content;
    [SerializeField] private Transform outputs_content;

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

        connectionInfoView_template.gameObject.SetActive(false);

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
        nodeName_text.text = node.GetData().name;
        nodeDesc_text.text = node.GetData().description;
        stats_productionTime_text.text = node.GetData().productionTime + "s";
        stats_successRate_text.text = "%" +node.GetData().successProbability * 100;
        stats_maintainCosts_text.text = "$" +node.GetData().maintainCost;

        icon_image.sprite = node.GetData().icon;
        iconBackground_image.sprite = node.GetData().icon;
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
        var usedPorts = new List<Port>();

        foreach (var recipe in currentNode.GetValidRecipes())
        {
            if (!usedRecipes.Contains(recipe.Key))
            {
                usedRecipes.Add(recipe.Key);

                foreach (var port in recipe.Value)
                {
                    usedPorts.Add(port);
                }

                var slot = Instantiate(recipeInformationView_template, recipes_content);
                slot.gameObject.SetActive(true);

                var ingredientsInfo = new List<RecipeInfo.IngredientInformation>();

                var invalidPorts = new List<IngredientData>();

                for (int i = 0; i < recipe.Key.GetIngredients().Length; i++)
                {
                    bool hasColor = false;
                    IngredientData data = recipe.Key.GetIngredients()[i].IngredientData;

                    foreach (var port in recipe.Value)
                    {
                        if (!invalidPorts.Contains(port.Product.data) && Recipe.CanBeUsedIn(port.Product.data, recipe.Key.GetIngredients()[i]))
                        {
                            hasColor = true;
                            invalidPorts.Add(port.Product.data);
                            data = port.Product.data;
                            break;
                        }
                    }

                    ingredientsInfo.Add(new RecipeInfo.IngredientInformation(data, hasColor));
                }

                slot.SetData(new RecipeInfo(ingredientsInfo.ToArray(), RecipeInfo.ConvertFrom(recipe.Key.GetResults(), true), true)); 
            }
        }
        
        var unnusedRecipes = currentNode.GetData().recipes.Where(x => !usedRecipes.Contains(x)).ToArray();
        var unnusedPorts = currentNode.GetInputPorts().Where(x => !usedPorts.Contains(x)).Select(x => x.Product.data).ToArray();

        foreach (var recipe in unnusedRecipes)
        {
            var slot = Instantiate(recipeInformationView_template, recipes_content);
            slot.gameObject.SetActive(true);

            var recipeIngredientInfo = new List<RecipeInfo.IngredientInformation>();
            var usedPortsInThisRecipe = new List<IngredientData>();

            for (int i = 0; i < recipe.GetIngredients().Length; i++)
            {
                bool hasColor = false;

                foreach (var ingredient in unnusedPorts)
                {
                    if (!usedPortsInThisRecipe.Contains(ingredient) && Recipe.CanBeUsedIn(ingredient, recipe.GetIngredients()[i]))
                    {
                        usedPortsInThisRecipe.Add(ingredient);
                        hasColor = true;
                        break;
                    }
                }

                recipeIngredientInfo.Add(new RecipeInfo.IngredientInformation(recipe.GetIngredients()[i].IngredientData, hasColor));
            }

            slot.SetData(new RecipeInfo(recipeIngredientInfo.ToArray(), RecipeInfo.ConvertFrom(recipe.GetResults(), false), false));
        }
    }

    private void DisplayConnectionsPage()
    {
        foreach (Transform child in inputs_content)
        {
            if (child.gameObject != connectionInfoView_template.gameObject)
            {
                Destroy(child.gameObject);
            }
        }

        foreach (Transform child in outputs_content)
        {
            Destroy(child.gameObject);
        }

        foreach (var inputPort in currentNode.GetInputPorts())
        {
            var slot = Instantiate(connectionInfoView_template, inputs_content);
            slot.gameObject.SetActive(true);
            slot.Display(inputPort, currentNode);
        }

        foreach (var outputPort in currentNode.GetOutputPorts())
        {
            if (outputPort.connection != null)
            {
                var slot = Instantiate(connectionInfoView_template, outputs_content);
                slot.gameObject.SetActive(true);
                slot.Display(outputPort, outputPort.connection.GetDestinationNode());
            }
        }
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
