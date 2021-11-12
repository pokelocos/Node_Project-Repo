using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NodeView : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer body;
    [SerializeField]
    private SpriteRenderer nodeIcon;
    [SerializeField]
    private SpriteRenderer fillBar;
    [SerializeField]
    private NodeData data;

    private float maxTime = 4f; //seconds
    private float actualTime = 0f;
    protected float internalSpeed = 0;

    protected Recipe[] recipes = new Recipe[0];

    protected ConectionView[] inputs;
    protected ConectionView[] outputs;

    protected Recipe selectedRecipe;

    private void Awake()
    {
        recipes = data.recipes;
        nodeIcon.sprite = data.icon;
        body.color = data.color;

        if(recipes[0].GetInputs().Length > 0)
            inputs = new ConectionView[recipes[0].GetInputs().Length];

        if (recipes[0].GetOutputs().Length > 0)
            outputs = new ConectionView[recipes[0].GetOutputs().Length];
    }

    public int GetMantainCost()
    {
        return data.maintainCost;
    }

    void Update()
    {
        Work();
    }

    private void Work()
    {
        actualTime += Time.deltaTime * data.speed * internalSpeed;
        if (actualTime > data.productionTime)
        {
            actualTime = 0;
            OnWorkFinish();
        }

        var radial = (1 - (actualTime / data.productionTime)) * 360;
        fillBar.GetComponent<SpriteRenderer>().material.SetFloat("_Arc2", radial);
    }

    public ConectionView[] GetInputs()
    {
        return inputs;
    }

    public ConectionView[] GetOutputs()
    {
        return outputs;
    }

    public Recipe GetCurrentRecipe()
    {
        return selectedRecipe;
    }

    public bool TryConnectWith(NodeView inputNode)
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
                                var connection = (Instantiate(Resources.Load("Nodes/Connection"), null) as GameObject).GetComponent<ConectionView>();

                                connection.SetNodes(this, inputNode);

                                inputNode.inputs[j] = connection;
                                outputs[i] = connection;

                                inputNode.ConnectionMade();
                                this.ConnectionMade();

                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }
    public static Ingredient[] GetInputIngredients(ConectionView[] inputs)
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


    public abstract void InputIngredientReady(ConectionView connection);

    protected abstract void ConnectionMade();

    protected abstract void OnWorkFinish();
}
