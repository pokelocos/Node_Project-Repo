using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Obsolete("this class is deprecated use 'EffectView' instead.")]
public class EffectView_Obs : MonoBehaviour 
{
    
    public string title = "Title";
    public string description = "Description lorem ipsum ameta no ate";

    [SerializeField]
    private Text title_text;
    [SerializeField]
    private Text desc_text;

    [SerializeField]
    private Image timerBar;

    private bool isPointerOver;

    public void SetData(string title, string desc)
    {
        this.title = title;
        this.description = desc;
    }

    void Update()
    {
        var rectTrans = GetComponent<RectTransform>();

        title_text.text = title;
        desc_text.text = description;

        if (isPointerOver)
        {
            rectTrans.sizeDelta = new Vector2(Mathf.Lerp(rectTrans.sizeDelta.x, 300, Time.unscaledDeltaTime * 7), 80);

            Color color = title_text.color;

            color = Color.Lerp(color, Color.white, Time.unscaledDeltaTime * 3);

            title_text.color = color;
            desc_text.color = color;
        }
        else
        {
            rectTrans.sizeDelta = new Vector2(Mathf.Lerp(rectTrans.sizeDelta.x, 80, Time.unscaledDeltaTime * 7), 80);

            Color color = title_text.color;

            color = Color.Lerp(color, Color.clear, Time.unscaledDeltaTime * 3);

            title_text.color = color;
            desc_text.color = color;
        }
    }

    public void SetFillAmount(float amount)
    {
        timerBar.fillAmount = amount;
    }

    public void OnPointerEnter()
    {
        isPointerOver = true;
    }

    public void OnPointerExit()
    {
        isPointerOver = false;
    }
}
