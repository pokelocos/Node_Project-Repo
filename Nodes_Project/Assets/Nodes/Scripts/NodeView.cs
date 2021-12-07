using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeView : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer body;
    [SerializeField]
    protected SpriteRenderer nodeIcon;
    [SerializeField]
    private SpriteRenderer fillBar;
    [SerializeField]
    private SpriteRenderer bright;
    [SerializeField]
    protected NodeData data;

    //Events
    public delegate void OnBarFilled();
    public event OnBarFilled onBarFilled;

    //Hidden variables
    private float maxTime = 4f; //seconds
    private float actualTime = 0f;
    protected float internalSpeed = 0;
    private bool isFailure;

    private void Awake()
    {
        string nameAux = data.name;
        data = Instantiate(data);
        data.name = nameAux;

        nodeIcon.sprite = data.icon;
        body.color = data.color;
    }

    public void SetInternalSpeed(float value)
    {
        internalSpeed = value;
    }

    public void AddInternalSpeed(float value)
    {
        internalSpeed += value;
    }

    public float GetInternalSpeed()
    {
        return internalSpeed;
    }

    public int GetMantainCost()
    {
        return data.maintainCost;
    }

    public Recipe[] GetRecipes()
    {
        return data.recipes;
    }

    public Sprite GetIcon()
    {
        return nodeIcon.sprite;
    }

    public string GetNodeName()
    {
        return data.name;
    }

    public string GetNodeDescription()
    {
        return "";
    }

    public Color GetColor()
    {
        return data.color;
    }

    public NodeData GetNodeData()
    {
        return data;
    }

    void Update()
    {
        switch (NodeManager.Filter)
        {
            case NodeManager.Filters.NONE:
                Paint(data.color);
                SetBright(Color.clear);
                break;
            case NodeManager.Filters.CONNECTION_MODE:
                Paint(Color.gray);
                break;
        }

        BarUpdate();
    }

    private void BarUpdate()
    {
        if (!isFailure)
        {
            actualTime += Time.deltaTime * data.speed * internalSpeed;

            fillBar.GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            actualTime -= Time.deltaTime * data.speed * internalSpeed;

            fillBar.GetComponent<SpriteRenderer>().color = Color.red;

            if (actualTime <= 0)
            {
                isFailure = false;

                actualTime = 0;
            }
        }

        if (actualTime > data.productionTime)
        {
            if (Random.Range(0, 1f) <= data.successProbability)
            {
                actualTime = 0;
                onBarFilled?.Invoke();
            }
            else
            {
                isFailure = true;
            }
        }

        var radial = (1 - (actualTime / data.productionTime)) * 360;
        fillBar.GetComponent<SpriteRenderer>().material.SetFloat("_Arc2", radial);
    }

    public void Paint(Color color)
    {
        body.color = color;
    }

    public void SetBright(Color color)
    {
        if (color == Color.clear)
        {
            bright.gameObject.SetActive(false);
            return;
        }

        bright.gameObject.SetActive(true);
        bright.color = color;
    }

    //public virtual RecipeInformationData[] GetRecipeInformationStatus()
    //{
    //    var recipesStatusData = new List<RecipeInformationData>();

    //    foreach (var recipe in GetRecipes())
    //    {
    //        var data = new RecipeInformationData(recipe);

    //        foreach (var input in GetInputs())
    //        {
    //            if(input != null && input.GetIngredient() != null)
    //            {
    //                for (int i = 0; i < data.inputsStatus.Count; i++)
    //                {
    //                    if (input.GetIngredient().ingredientName == data.inputsStatus[i].ingredient.ingredientName)
    //                    {
    //                        if (data.inputsStatus[i].status == false)
    //                        {
    //                            data.inputsStatus[i] = new RecipeInformationData.IngredientStatus(input.GetIngredient(), true);
    //                            break;
    //                        }
    //                    }
    //                }
    //            }
    //        }

    //        bool canCraft = true;

    //        foreach (var ingredient in data.inputsStatus)
    //        {
    //            if (ingredient.status == false)
    //            {
    //                canCraft = false;
    //                break;
    //            }
    //        }

    //        data.canCraft = canCraft;

    //        recipesStatusData.Add(data);
    //    }

    //    return recipesStatusData.ToArray();
    //}
}

public class RecipeInformationData
{
    public Recipe originalRecipe;

    public struct IngredientStatus
    {
        public IngredientData ingredient;
        public bool status;

        public IngredientStatus(IngredientData ingredient, bool status)
        {
            this.ingredient = ingredient;
            this.status = status;
        }
    }

    public List<IngredientStatus> inputsStatus = new List<IngredientStatus>();

    public bool canCraft;

    public RecipeInformationData(Recipe originalRecipe)
    {
        this.originalRecipe = originalRecipe;

        foreach (var input in originalRecipe.GetInputs())
        {
            inputsStatus.Add(new IngredientStatus(input, false));
        }
    }
}
