using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class VersionTracker : MonoBehaviour
{
    public string prefix = "";
    public string suffix = "";

    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    void Start()
    {
        text.text = prefix + Application.version + suffix;
    }

}
