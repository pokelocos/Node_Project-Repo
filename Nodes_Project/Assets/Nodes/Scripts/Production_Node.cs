using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Production_Node : NodeView
{
    private void Start()
    {
        internalSpeed = 1;

        //The production Nodes should have only one recipe with only outputs.
        selectedRecipe = recipes[0];
    }
    protected override void OnWorkFinish()
    {
        foreach (var output in outputs)
        {
            output?.SendIngredient();
        }
    }

    protected override void ConnectionMade()
    {
       
    }

    public override void InputIngredientReady(ConectionView connection)
    {
       
    }
}
