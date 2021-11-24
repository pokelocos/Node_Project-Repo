using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Supermarket_Node : NodeView
{
    Ingredient[] ingredients;

    private void Start()
    {
        inputs = new ConectionView[4];
    }

    protected override void OnWorkFinish()
    {
        internalSpeed = 0;

        var money = 0;

        foreach (var input in inputs)
        {
            if (input != null)
            {
                var ingredient = input.GetOutputIngredient();

                if (ingredient != null)
                {
                    if (ingredient.name != "Canned Food")
                    {
                        money += Mathf.CeilToInt(ingredient.price * 0.3f);
                    }
                    else
                    {
                        money += ingredient.price;
                    }
                }

                input.isReadyToClaim = false;
            }
        }

        bool success = false;

        for (int i = 0; i < inputs.Length; i++)
        {
            if (inputs[i] != null)
            {
                success = true;
                break;
            }

        }

        if (success)
            GetComponent<Animator>().SetTrigger("Success");
        else
            GetComponent<Animator>().SetTrigger("Fail");

        GameManager.AddMoney(money);
    }

    public override int CanConnectWith(NodeView inputNode)
    {
        return 0;
    }

    public override void ConnectionChange()
    {

    }

    public override void InputIngredientReady(ConectionView connection)
    {
        var readyforSell = inputs.Count(x => x != null && x.isReadyToClaim);

        if (readyforSell > 0)
            internalSpeed = 1;
    }
}
