using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Packaging_Node : Factory_Node
{
    private void Start()
    {
        inputs = new ConectionView[3];

        selectedRecipe = recipes[0];
    }
    public override void ConnectionChange()
    {

    }

    public override void InputIngredientReady(ConectionView connection)
    {
        foreach (var input in inputs)
        {
            if (input != null)
            {
                if (!input.isReadyToClaim)
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

        foreach (var input in inputs)
        {
           if(input != null)
                input.isReadyToClaim = false;
        }

        if (selectedRecipe != null)
            internalSpeed = 1;
    }
}