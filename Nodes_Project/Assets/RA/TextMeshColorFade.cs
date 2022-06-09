using System.Collections;
using UnityEngine;

public class TextMeshColorFade : MonoBehaviour
{
    [SerializeField] private float alpha = 0;

    [SerializeField] private TextMesh text;
    [SerializeField] private TextMesh text_shadow;

    Color color;
    Color colorShadow;

    // Use this for initialization
    void Start()
    {
        color = text.color;
        colorShadow = text_shadow.color;
    }

    // Update is called once per frame
    void Update()
    {
        color.a = alpha;
        colorShadow.a = alpha / 2;

        text.color = color;
        text_shadow.color = colorShadow;
    }
}