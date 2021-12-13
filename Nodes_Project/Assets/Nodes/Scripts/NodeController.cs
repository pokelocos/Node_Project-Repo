using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using RA.InputManager;

[RequireComponent(typeof(NodeView))]
public class NodeController : MonoBehaviour, SelectableObject
{
    public bool isDebugMode;

    private Dictionary<Recipe, Port[]> selectedRecipes = new Dictionary<Recipe, Port[]>();
    private List<Port> inputPorts = new List<Port>();
    private List<Port> outputPorts = new List<Port>();

    private ProductionReport[] lasProductionManifest;

    private NodeView nodeView;

    private List<ProductionQueue> productionQueue = new List<ProductionQueue>();

    public NodeView NodeView { get => nodeView; private set => nodeView = value; }

    public struct ProductionQueue
    {
        public Recipe recipe;
        public Port[] inputPorts;
        public Port[] outputPorts;

        public ProductionQueue(Recipe recipe, Port[] inputPorts, Port[] outputPorts)
        {
            this.recipe = recipe;
            this.inputPorts = inputPorts;
            this.outputPorts = outputPorts;
        }
    }

    private void Awake()
    {
        nodeView = GetComponent<NodeView>();

        nodeView.onBarFilled += OnWorkFinish;
    }

    void Start()
    {
        InitializeDefaultInputPorts();

        //Execute all actions on initialize node
        foreach (var action in nodeView.GetNodeData().onInitialize)
        {
            action.CallAction(this);
        }

        CheckWhatCanCraft();
    }

    public ProductionQueue[] GetProductionQueue()
    {
        return productionQueue.ToArray();
    }

    private void InitializeDefaultInputPorts()
    {
        foreach (var ingredient in nodeView.GetNodeData().defaultInputIngredients)
        {
            AddInput(new Port(null, new Product(ingredient, ingredient.price,  new Product[0])));
        }
    }

    public void OnInputPortReceiveProduct(Port port)
    {
        port.isProductInPort = true;

        UpdateProductionQueue();
    }

    private void OnWorkFinish()
    {
        //Execute all actions when the node fill his bar.
        foreach (var queue in productionQueue)
        {
            foreach (var input in queue.inputPorts)
            {
                input.isProductInPort = false;
                input.connection?.UseConnection();
            }

            var productionHistorial = new List<ProductionReport>();

            foreach (var output in queue.outputPorts)
            { 
                productionHistorial.Add(new ProductionReport(output.Product, output.connection != null));

                output.connection?.SendProduct();
            }

            lasProductionManifest = productionHistorial.ToArray();
        }

        foreach (var action in nodeView.GetNodeData().onWorkFinish)
        {
            action.CallAction(this);
        }

        if (productionQueue.Count == 0)
        {
            nodeView.SetInternalSpeed(0);
        }
    }

    public ProductionReport[] GetProductionManifest()
    {
        return lasProductionManifest;
    }

    private void UpdateProductionQueue()
    {
        productionQueue.Clear();

        var workingPorts = new List<Port>();

        //Check if can produce eny products
        foreach (var selectedRecipe in selectedRecipes)
        {
            var usedPorts = new List<Port>();
            //Determine if this recipe have all his input ports ready for collect.
            bool canProduce = selectedRecipe.Value.All(x => x.isProductInPort);

            if (canProduce)
            {
                //Find the output nodes
                foreach (var result in selectedRecipe.Key.GetResults())
                {
                    foreach (var outputPort in outputPorts)
                    {
                        if (outputPort.Product.data == result)
                        {
                            if (!workingPorts.Contains(outputPort))
                            {
                                workingPorts.Add(outputPort);
                                usedPorts.Add(outputPort);
                            }
                        }
                    }
                }

                productionQueue.Add(new ProductionQueue(selectedRecipe.Key, selectedRecipe.Value, usedPorts.ToArray()));
            }
        }

        if (productionQueue.Count > 0)
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
            var rawMaterials = recipe.Value.Select(x => x.Product).ToArray();

            //Set output products
            allOutputProducts.AddRange(recipe.Key.GenerateProducts(rawMaterials, Mathf.CeilToInt(nodeView.GetNodeData().maintainCost / 4)));
        }

        var occupiedProducts = new List<Product>();
        var occupiedPorts = new List<Port>();

        //Check if existing ports are used.
        foreach (var product in allOutputProducts)
        {
            //For each port that is not contained in the list of used ports
            foreach (var outputPort in outputPorts.Where(x => !occupiedPorts.Contains(x)))
            {
                if (product.data == outputPort.Product.data)
                {
                    occupiedProducts.Add(product);
                    occupiedPorts.Add(outputPort);
                    break;
                }
            }
        }

        allOutputProducts = allOutputProducts.Except(occupiedProducts).ToList();

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
        selectedRecipes.Clear();

        NodeData data = nodeView.GetNodeData();

        string log =  "[Check crafting options for " + gameObject.name + "]\n [Combinations]\n";

        //Calculate all possible combinations
        var inputIngredientPorts = inputPorts.Where(x => x.Product?.data).ToList();

        List<List<Port>> allCombinations = new List<List<Port>>();

        for (int i = 1; i <= inputIngredientPorts.Count; i++)
        {
            List<List<Port>> combinations = Combinations<Port>.GetCombinations(inputIngredientPorts, i);

            allCombinations.AddRange(combinations);
        }

        //Generate the list of possible Recipes
        var validRecipes = new Dictionary<Recipe, List<Port>>();

        foreach (var combination in allCombinations)
        {
            log += "======= " + Product.ListToString(combination) + "=========\n";
            foreach (var recipe in data.recipes)
            {
                log += Product.ListToString(combination) + " =";

                bool validRecipe = true;
                var ingredients = recipe.GetIngredients().OrderByDescending(x => !x.IsOptional).ToList();
                var validPorts = new List<Port>();

                foreach (var candidate in combination)
                {
                    if (ingredients.Count > 0)
                    {
                        Ingredient matchIngredient = null;

                        if (Recipe.CanBeUsedIn(candidate.Product.data, ingredients.ToArray(), out matchIngredient))
                        {
                            ingredients.Remove(matchIngredient);
                            validPorts.Add(candidate);
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

                //Check if unnused items are optionals
                foreach (var ingredient in ingredients)
                {
                    if (!ingredient.IsOptional)
                        validRecipe = false;
                }

                if (validRecipe && !validRecipes.ContainsKey(recipe))
                {
                    validRecipes.Add(recipe, validPorts);

                    log += recipe.name + "\n";

                    break;
                }
                else
                {
                    log += "RECIPE NOT FOUND\n";
                }
            }
        }

        //Select the best recipes
        validRecipes = validRecipes.OrderByDescending(x => x.Key.GetIngredients().Length).ToDictionary(x => x.Key, y => y.Value); //Order by recipe input size

        foreach (var recipe in validRecipes)
        {
            var recipeIngredients = recipe.Key.GetIngredients().Select(x => x.IngredientData).ToArray();

            if (selectedRecipes.Count < nodeView.GetNodeData().allowedRecipes)
            {
                selectedRecipes.Add(recipe.Key, recipe.Value.ToArray());
            }
        }

        //Apply Buffs with the rest of the unnused ingredients

        UpdatePorts();
        UpdateProductionQueue();

        if (isDebugMode)
            Debug.Log(log);
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
    /// Returns all the connected input ports.
    /// </summary>
    /// <returns></returns>
    public NodeController[] GetConnectedInputNodes()
    {
        return inputPorts.Select(x => x.connection?.GetOrigin()).ToArray();
    }

    /// <summary>
    /// Returns all the connected output ports.
    /// </summary>
    /// <returns></returns>
    public NodeController[] GetConnectedOutputNodes()
    {
        return outputPorts.Select(x => x.connection?.GetDestination()).ToArray();
    }

    /// <summary>
    /// Evaluate if this node can connect with another node. Returns:
    /// 0 - Can't connect
    /// 1 - Can Connect
    /// 2 - It's possible but the input slots are full
    /// 3 - Can't connect because this nodes are alredy connected.
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
            return 3;
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
                if (candidate.GetInputPorts().Length < 8)
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

    public static string ListToString(IEnumerable<Port> list)
    {
        string text = string.Empty;

        foreach (var port in list)
        {
            text += port.Product.data.ingredientName + " + ";
        }

        return text;
    }
}

/// <summary>
/// This class its used to store the production information.
/// </summary>
public class ProductionReport
{
    private Product product;
    private bool wasDispatched;

    public Product Product
    {
        get
        {
            return product;
        }
    }

    public bool WasDispatched
    {
        get
        {
            return wasDispatched;
        }
    }

    public ProductionReport(Product product, bool wasDispatched)
    {
        this.product = product;
        this.wasDispatched = wasDispatched;
    }
}