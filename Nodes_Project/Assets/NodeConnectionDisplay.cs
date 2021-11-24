using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NodeConnectionDisplay : MonoBehaviour
{
    [SerializeField] private Text inputText;
    [SerializeField] private Text outputText;

    public void SetInputsText(string input)
    {
        inputText.text = input;
    }

    public void SetOutputsText(string output)
    {
        outputText.text = output;
    }
}
