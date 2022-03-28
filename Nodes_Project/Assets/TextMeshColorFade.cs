using System.Collections;
using UnityEngine;

public class TextMeshColorFade : MonoBehaviour
{
    [SerializeField] private float moneyAlpha = 0;

    [SerializeField] private TextMesh price;
    [SerializeField] private TextMesh price_shadow;

    Color moneyFeedback;
    Color moneyShadowFeedback;

    // Use this for initialization
    void Start()
    {
        moneyFeedback = price.color;
        moneyShadowFeedback = price_shadow.color;
    }

    // Update is called once per frame
    void Update()
    {
        moneyFeedback.a = moneyAlpha;
        moneyShadowFeedback.a = moneyAlpha / 2;

        price.color = moneyFeedback;
        price_shadow.color = moneyShadowFeedback;
    }
}