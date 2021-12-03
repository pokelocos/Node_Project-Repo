using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NodeView : MonoBehaviour
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

    public bool isDebugMode;

    public bool isActive = true;

    private float maxTime = 4f; //seconds
    private float actualTime = 0f;
    protected float internalSpeed = 0;
    private bool isFailure;

    protected ConnectionView[] inputs = new ConnectionView[0];
    protected ConnectionView[] outputs = new ConnectionView[0];

    protected Recipe selectedRecipe;

    private void Awake()
    {
        string nameAux = data.name;
        data = Instantiate(data);
        data.name = nameAux;

        nodeIcon.sprite = data.icon;
        body.color = data.color;

        if(data.recipes[0].GetInputs().Length > 0)
            inputs = new ConnectionView[data.recipes[0].GetInputs().Length];

        if (data.recipes[0].GetOutputs().Length > 0)
            outputs = new ConnectionView[data.recipes[0].GetOutputs().Length];
    }

    public int GetMantainCost()
    {
        return data.maintainCost;
    }

    public Recipe[] GetRecipes()
    {
        return data.recipes;
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

        //if (isActive)
        //{
        //    Paint(GetColor());
        //}
        //else
        //{
        //    Paint(Color.gray);
        //}

        Work();
    }

    private void Work()
    {
        if (!isFailure)
        {
            actualTime += Time.deltaTime * data.speed * internalSpeed * System.Convert.ToInt32(isActive);

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
            if (Random.Range(0,1f) <= data.successProbability)
            {
                actualTime = 0;
                OnWorkFinish();
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

    public void RemoveConnection(ConnectionView connection)
    {
        for (int i = 0; i < inputs.Length; i++)
        {
            if (inputs[i] != null && inputs[i] == connection)
            {
                inputs[i] = null;
                return;
            }
        }

        for (int i = 0; i < outputs.Length; i++)
        {
            if (outputs[i] != null && outputs[i] == connection)
            {
                outputs[i] = null;
                return;
            }
        }
    }

    public ConnectionView[] GetInputs()
    {
        return inputs;
    }

    public ConnectionView[] GetOutputs()
    {
        return outputs;
    }

    public Recipe GetCurrentRecipe()
    {
        return selectedRecipe;
    }

    [System.Obsolete("This is an obsolete method")]
    /// <summary>
    /// Returns:
    ///  0 - Cant connect
    ///  1 - Posible connection
    ///  2 - Satisfactory connection
    /// </summary>
    /// <param name="nodeView"></param>
    /// <returns></returns>
    public virtual int CanConnectWith(NodeView inputNode)
    {
        return 0;

        if (outputs.Length > 0)
        {
            for (int i = 0; i < outputs.Length; i++)
            {
                if (outputs[i] == null)
                {
                    if (inputNode.inputs.Length > 0)
                    {
                        for (int j = 0; j < inputNode.inputs.Length; j++)
                        {
                            if (inputNode.inputs[j] == null)
                            {
                                return 2;
                            }
                        }
                    }
                }
            }
        }

        return 0;
    }

    [System.Obsolete("This is an obsolete method")]
    public abstract Recipe[] ValidRecipes();

    [System.Obsolete("This is an obsolete method")]
    public void ConnectWith(NodeView inputNode)
    {
        if (outputs.Length > 0)
        {
            for (int i = 0; i < outputs.Length; i++)
            {
                if (outputs[i] == null)
                {
                    if (inputNode.inputs.Length > 0)
                    {
                        for (int j = 0; j < inputNode.inputs.Length; j++)
                        {
                            if (inputNode.inputs[j] == null)
                            {
                                var connection = (Instantiate(Resources.Load("Nodes/Connection"), null) as GameObject).GetComponent<ConnectionView>();

                                inputNode.inputs[j] = connection;
                                outputs[i] = connection;

                                connection.SetNodes(this, inputNode);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
    public static Ingredient[] GetInputIngredients(ConnectionView[] inputs)
    {
        List<Ingredient> ingredients = new List<Ingredient>();

        foreach (var input in inputs)
        {
            if (input != null)
            {
                for (int i = 0; i < input.GetOrigin().GetOutputs().Length; i++)
                {
                    if (input.GetOrigin().GetOutputs()[i] != null)
                    {
                        if (input.GetOrigin().GetOutputs()[i] == input)
                        {
                            if (input.GetOrigin().GetCurrentRecipe() != null)
                            {
                                if (input.GetOrigin().GetCurrentRecipe().GetOutputs()[i] != null)
                                {
                                    ingredients.Add(input.GetOrigin().GetCurrentRecipe().GetOutputs()[i]);
                                }
                            }

                            break;
                        }
                    }
                }
            }
        }

        return ingredients.ToArray();
    }

    public abstract void InputIngredientReady(ConnectionView connection);

    public abstract void ConnectionChange();

    protected abstract void OnWorkFinish();

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

    public int GetConnectedInputs()
    {
        int a = 0;
        foreach(ConnectionView c in inputs)
        {
            if (c != null)
                a++;
        }
        return a;
    }

    public int GetConnectedOutputs()
    {
        int a = 0;
        foreach (ConnectionView c in outputs)
        {
            if (c != null)
                a++;
        }
        return a;
    }

    private void OnGUI()
    {
        if (Application.isEditor && isDebugMode)
        {
            DebugInfo();
        }
    }

    protected virtual void DebugInfo()
    {
        Vector3 offset = new Vector3(-5, -0.5f, 0);

        var screenPos = Camera.main.WorldToScreenPoint(transform.position + offset);

        screenPos.y = Screen.height - screenPos.y - 50;

        string inputLog = "NO INPUTS";

        if (inputs.Length > 0)
        {
            inputLog = string.Empty;

            foreach (var input in inputs)
            {
                if (input == null)
                {
                    inputLog += "[EMPTY]";
                }
                else
                {
                    inputLog += "[" + input.GetIngredient().ingredientName + "]";
                }

                inputLog += "\n";
            }
        }

        string outputLog = "NO OUTPUTS";

        if (outputs.Length > 0)
        {
            outputLog = string.Empty;

            foreach (var output in outputs)
            {
                if (output == null)
                {
                    outputLog += "[EMPTY]";
                }
                else
                {
                    outputLog += "[" + output.GetIngredient().ingredientName + "]";
                }

                outputLog += "\n";
            }
        }

        GUI.Box(new Rect(screenPos.x, screenPos.y, 70, 70), inputLog);

        GUI.Box(new Rect(screenPos.x + 180, screenPos.y, 70, 70), outputLog);
    }

    public virtual RecipeInformationData[] GetRecipeInformationStatus()
    {
        var recipesStatusData = new List<RecipeInformationData>();

        foreach (var recipe in GetRecipes())
        {
            var data = new RecipeInformationData(recipe);

            foreach (var input in GetInputs())
            {
                if(input != null && input.GetIngredient() != null)
                {
                    for (int i = 0; i < data.inputsStatus.Count; i++)
                    {
                        if (input.GetIngredient().ingredientName == data.inputsStatus[i].ingredient.ingredientName)
                        {
                            if (data.inputsStatus[i].status == false)
                            {
                                data.inputsStatus[i] = new RecipeInformationData.IngredientStatus(input.GetIngredient(), true);
                                break;
                            }
                        }
                    }
                }
            }

            bool canCraft = true;

            foreach (var ingredient in data.inputsStatus)
            {
                if (ingredient.status == false)
                {
                    canCraft = false;
                    break;
                }
            }

            data.canCraft = canCraft;

            recipesStatusData.Add(data);
        }

        return recipesStatusData.ToArray();
    }
}

public class RecipeInformationData
{
    public Recipe originalRecipe;

    public struct IngredientStatus
    {
        public Ingredient ingredient;
        public bool status;

        public IngredientStatus(Ingredient ingredient, bool status)
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
