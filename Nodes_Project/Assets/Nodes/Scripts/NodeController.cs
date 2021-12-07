using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RA.InputManager;

[RequireComponent(typeof(NodeView))]
public class NodeController : MonoBehaviour, SelectableObject
{
    private List<Recipe> selectedRecipes = new List<Recipe>();
    private List<Port> inputPorts = new List<Port>();
    private List<Port> outputPorts = new List<Port>();

    private NodeView nodeView;

    private void Awake()
    {
        nodeView = GetComponent<NodeView>();

        nodeView.onBarFilled += OnWorkFinish;
    }

    void Start()
    {
        InitializeDefaultInputPorts();
        InitializeDefaultRecipes();
        CheckWhatCanCraft();
    }

    private void InitializeDefaultInputPorts()
    {
        foreach (var ingredient in nodeView.GetNodeData().defaultInputIngredients)
        {
            AddInput(new Port(null, new Product(ingredient, ingredient.price,  new Product[0])));
        }
    }

    private void InitializeDefaultRecipes()
    {
        if (nodeView.GetNodeData().forceRecipes)
        {
            selectedRecipes.Capacity = nodeView.GetNodeData().allowedRecipes;
            selectedRecipes.AddRange(nodeView.GetNodeData().recipes);
        }
    }

    private void OnWorkFinish()
    {
        //Execute all actions when the node fill his bar.
        foreach (var action in nodeView.GetNodeData().onWorkFinish)
        {
            action.CallAction(this);
        }

        //Get reached ingredients
        var reachedPorts = new List<Port>();

        foreach (var port in inputPorts)
        {
            if (port.Product != null && port.isProductInPort)
            {
                reachedPorts.Add(port);
            }
        }

     //   print(nodeView.name + " = " + reachedPorts.Count);

        //Check what ingredients can be used in some recipe
        foreach (var recipe in selectedRecipes)
        {
            var requiredIngredients = new List<Port>();

            foreach (var reachedIngredient in reachedPorts)
            {
                if (reachedIngredient.isProductInPort)
                {
                    if (recipe.CanBeUsedIn(reachedIngredient.Product.data))
                    {
                        requiredIngredients.Add(reachedIngredient);
                    }

                    //If all the required ingredients are found
                    if (recipe.CanBeUsedIn(requiredIngredients.ToArray()))
                    {
                        foreach (var port in requiredIngredients)
                        {
                            reachedIngredient.isProductInPort = false;
                            port.connection.UseConnection();
                        }

                        foreach (var outputData in recipe.GetResults())
                        {
                            var port = FindOutput(outputData);

                            if (port != null && port.connection != null)
                            {
                                port.connection.SendProduct();
                            }
                        }

                        break;
                    }
                }
            }
        }

        if (selectedRecipes.Count > 0)
        {
            nodeView.SetInternalSpeed(1);
        }
    }

    /// <summary>
    /// Verify and update ports.
    /// </summary>
    public void UpdatePorts()
    {
        //Collect all output ingredients
        var allOutputProducts = new List<Product>();

        foreach (var recipe in selectedRecipes)
        {
            var rawMaterials = new List<Product>();

            foreach (var ingredient in recipe.GetIngredients())
            {
                foreach (var port in inputPorts)
                {
                    if (ingredient.IngredientData == port.Product.data)
                    {
                        rawMaterials.Add(port.Product);
                        break;
                    }
                }
            }

            //Set output products
            allOutputProducts.AddRange(recipe.GenerateProducts(rawMaterials.ToArray(), Mathf.CeilToInt(nodeView.GetNodeData().maintainCost / 4)));
        }

        var toRemove = new List<Product>();
        var occupiedPorts = new List<Port>();

        //Check if existing ports are used.
        foreach (var product in allOutputProducts)
        {
            //For each port that is not contained in the list of used ports
            foreach (var outputPort in outputPorts.Where(x => !occupiedPorts.Contains(x)))
            {
                if (product.data == outputPort.Product.data)
                {
                    toRemove.Add(product);
                    occupiedPorts.Add(outputPort);
                    break;
                }
            }
        }

        allOutputProducts = allOutputProducts.Except(toRemove).ToList();

        //If some output ingredients still remaining create new ones.

        if (allOutputProducts.Count > 0)
        {
            foreach (var product in allOutputProducts)
            {
                Port port = new Port(null, product);
                outputPorts.Add(port);
                occupiedPorts.Add(port);
            }
        }

        //Destroy the ports who doesnt exist in occupiedPorts
        var unnusedPorts = outputPorts.Where(x => !occupiedPorts.Contains(x)).ToArray();

        foreach (var port in unnusedPorts)
        {
            port.connection?.Disconnect();
        }

        outputPorts.RemoveAll(x => !occupiedPorts.Contains(x));
    }

    /// <summary>
    /// This method it's called after connection was made.
    /// </summary>
    /// <param name="connection"></param>
    public void ConnectionUpdated(ConnectionView connection)
    {
        CheckWhatCanCraft();
    }

    /// <summary>
    /// Update the selected recipe list.
    /// </summary>
    private void CheckWhatCanCraft()
    {
        if (!nodeView.GetNodeData().forceRecipes)
        {
            selectedRecipes.Clear();

            NodeData data = nodeView.GetNodeData();

            //Calculate all possible combinations
            var inputIngredientDatas = inputPorts.Select(x => x.Product?.data).ToList();

            List<List<IngredientData>> allCombinations = new List<List<IngredientData>>();

            for (int i = 1; i <= inputIngredientDatas.Count; i++)
            {
                List<List<IngredientData>> combinations = Combinations<IngredientData>.GetCombinations(inputIngredientDatas, i);

                allCombinations.AddRange(combinations);
            }

            //Generate the list of possible Recipes
            var validRecipes = new List<Recipe>();

            foreach (var combination in allCombinations)
            {
                // print(nodeView.GetNodeData().name + " = " + combination[0]);

                foreach (var recipe in data.recipes)
                {
                    bool validRecipe = true;
                    var ingredients = recipe.GetIngredients().ToList();

                    foreach (var ingredient in combination)
                    {
                        if (ingredients.Count > 0)
                        {
                            Ingredient matchIngredient = null;

                            if (Recipe.CanBeUsedIn(ingredient, ingredients.ToArray(), out matchIngredient))
                            {
                                ingredients.Remove(matchIngredient);
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

                    if (validRecipe)
                        validRecipes.Add(recipe);
                }
            }

            //Select the best recipes
            validRecipes = validRecipes.OrderByDescending(x => x.GetIngredients().Length).ToList(); //Order by recipe input size

            foreach (var recipe in validRecipes)
            {
                var recipeIngredients = recipe.GetIngredients().Select(x => x.IngredientData).ToArray();

                if (recipeIngredients.All(value => inputIngredientDatas.Contains(value)))
                {
                    selectedRecipes.Add(recipe);
                }
            }
        }

        if (selectedRecipes.Count > 0)
        {
            nodeView.SetInternalSpeed(1);
        }

        //Apply Buffs with the rest of the unnused ingredients

        UpdatePorts();
    }

    public Port GetOutputPort(ConnectionController connection)
    {
        return outputPorts.First(x => x.connection == connection);
    }

    public Port GetOutputPort(Product product)
    {
        return outputPorts.First(x => x.Product == product);
    }

    public Port GetInputPort(ConnectionController connection)
    {
        return inputPorts.First(x => x.connection == connection);
    }

    public Port GetInputPort(Product product)
    {
        return inputPorts.First(x => x.Product == product);
    }

    public Port[] GetInputPorts()
    {
        return inputPorts.ToArray();
    }

    public Port[] GetOutputPorts()
    {
        return outputPorts.ToArray();
    }

    public void AddInput(Port port)
    {
        inputPorts.Add(port);
    }

    public Port FindOutput(IngredientData filter)
    {
        foreach (var port in outputPorts)
        {
            if (port.Product != null && port.Product.data == filter)
            {
                return port;
            }
        }

        return null;
    }

    public void SetOutput(ConnectionController connection)
    {
        var port = GetFreeOutput();

        if (port != null)
        {
            port.connection = connection;
        }
    }

    public void RemoveInput(ConnectionController connection)
    {
        inputPorts.Remove(inputPorts.Find(x => x.connection == connection));
    }

    public void RemoveOutput(ConnectionController connection)
    {
        outputPorts.Remove(outputPorts.Find(x => x.connection == connection));
    }

    /// <summary>
    /// Evaluate if this node can connect with another node. Returns:
    /// 0 - Can't connect
    /// 1 - Can Connect
    /// 2 - It's possible but not right now.
    /// </summary>
    /// <param name="candidate"></param>
    /// <param name="ingredient"></param>
    /// <returns></returns>
    public int CanConnectWith(NodeController candidate, Product product)
    {
        //Check if this node is not the same
        if (this == candidate)
        {
            return 0;
        }

        //Check if the candidate are not connected twice
        if (this.IsConnectedWith(candidate))
        {
            return 0;
        }

        //Check if the prduct exists
        if (product == null)
        {
            return 0;
        }

        foreach (var recipe in candidate.nodeView.GetNodeData().recipes)
        {
            if (recipe.CanBeUsedIn(product.data))
            {
                if (inputPorts.Count < 8)
                {
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
        }

        return 0;
    }

    /// <summary>
    /// Get the next unconnected output. Returns null if no unconnected ports are found.
    /// </summary>
    /// <returns></returns>
    public Port GetFreeOutput()
    {
        foreach (var output in outputPorts)
        {
            if (output.connection == null)
            {
                return output;
            }
        }

        return null;
    }

    /// <summary>
    /// Returns true if this node is alredy connected with other node.
    /// </summary>
    /// <returns></returns>
    public bool IsConnectedWith(NodeController other)
    {
        foreach (var outputPort in outputPorts)
        {
            if (outputPort.connection != null)
            {
                foreach (var otherInputPort in other.inputPorts)
                {
                    if (outputPort.connection.Equals(otherInputPort.connection))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}

[System.Serializable]
public class Port
{
    public ConnectionController connection;
    private Product product;
    public bool isProductInPort;

    public Product Product
    {
        get
        {
            return product;
        }
    }

    public Port(ConnectionController connection, Product product)
    {
        this.connection = connection;
        this.product = product;
    }
}

[System.Serializable]
public class Product
{
    public IngredientData data;
    public int currentValue;
    public Product[] ingredients;

    public Product(IngredientData data, int currentValue, Product[] ingredients)
    {
        this.data = data;
        this.currentValue = currentValue;
        this.ingredients = ingredients;
    }
}