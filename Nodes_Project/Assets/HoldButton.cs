using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Obsolete]
public class HoldButton : MonoBehaviour
{
    public UnityEvent evnt;

    public float maxTime;
    private float currentTime = 0f;

    public Image img;

    private bool boleano;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(boleano)
        {
            currentTime += Time.deltaTime;
            img.fillAmount = currentTime / maxTime;

            if (currentTime / maxTime >= 1)
            {
                evnt?.Invoke();
                ResetTime();
            }
        }

    }

    public void StartTime()
    {
        boleano = true;
    }

    public void EndTime()
    {
        boleano = false;
        ResetTime();
    }

    public void ResetTime()
    {
        currentTime = 0;
        img.fillAmount = 0;
    }
}
