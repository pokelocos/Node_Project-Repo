using System;
using UnityEngine;
using UnityEngine.UI;

public class GuiManager : MonoBehaviour
{
    [SerializeField] private Text money_text;
    [SerializeField] private Text days_text;
    [SerializeField] private Text balance_text;
    [SerializeField] private Text starAmount_text;
    [SerializeField] private GameObject pause_frame;

    private float balance_alpha = 0; //?
    private Color balance_color = Color.green; // balanceManager (?)

    public void Update()
    {
        pause_frame.gameObject.SetActive(Time.timeScale == 0);

        balance_color.a = balance_alpha;
        if (balance_alpha > 0)
        {
            balance_alpha -= Time.deltaTime;
        }
        balance_text.color = balance_color;
    }

    internal void SetBalance(int balance)
    {
        if (balance > 0)
        {
            balance_text.text = "$" + balance;
            balance_color = Color.green;
        }
        else
        {
            balance_text.text = "$" + balance;
            balance_color = Color.red;
        }

        balance_alpha = 5;
    }

    internal void SetDay(int day)
    {
        days_text.text = day.ToString();
    }

    internal void SetContract(int current, int max)
    {
        starAmount_text.text = current + "/" + max; 
    }

    internal void SetMoneyValue(int v) 
    {
        if (v < 0)
        {
            money_text.color = new Color(251f / 255f, 181f / 255f, 181f / 255f);
            money_text.text = "-$" + (-v);
        }
        else
        {
            money_text.color = Color.white;
            money_text.text = "$" + v;
        }
    }
}

