using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu]
public class Recipe : ScriptableObject
{
    [SerializeField] Ingredient[] inputs;
    [SerializeField] Ingredient[] outputs;

    public Ingredient[] GetOutputs()
    {
        return outputs;
    }

    public Ingredient[] GetInputs()
    {
        return inputs;
    }

    public bool HasInputIngredients(Ingredient[] listToCheck)
    {
        foreach(Ingredient ingredient in inputs)
        {
            if(!listToCheck.Contains(ingredient))
            {
                return false;
            }
        }
        return true;
    }

}
