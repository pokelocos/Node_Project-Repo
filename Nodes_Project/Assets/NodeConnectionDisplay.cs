using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeConnectionDisplay : MonoBehaviour
{
    [SerializeField] private GameObject inputObject;
    [SerializeField] private GameObject outputObject;

    public void SetInputsText(string input)
    {
        inputObject.GetComponentInChildren<Text>().text = input;
    }

    public void SetOutputsText(string output)
    {
        outputObject.GetComponentInChildren<Text>().text = output;
    }

    public void EnableIOObjetcs(Recipe[] recipes)
    {
        int inputs = 0, outputs = 0;
        foreach(Recipe r in recipes)
        {
            foreach(Ingredient i in r.GetInputs())
            {
                if(!(i.name == "$" || i.name == "$$" || i.name == "Time"))
                {
                    inputs++;
                }
            }
            foreach (Ingredient i in r.GetOutputs())
            {
                if (!(i.name == "$" || i.name == "$$" || i.name == "Time"))
                {
                    outputs++;
                }
            }
        }
        inputObject.SetActive(inputs > 0);
        outputObject.SetActive(outputs > 0);
    }
}
