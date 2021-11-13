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

    public int InputIngredientsMatchCount(Ingredient[] ingredients)
    {
        int matches = 0;

        foreach (var ingredient in ingredients)
        {
            if (inputs.Contains(ingredient))
                matches++;
        }

        return matches;
    }

    public int OutputIngredientsMatchCount(Ingredient[] ingredients)
    {
        int matches = 0;

        foreach (var ingredient in ingredients)
        {
            if (outputs.Contains(ingredient))
                matches++;
        }

        return matches;

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
