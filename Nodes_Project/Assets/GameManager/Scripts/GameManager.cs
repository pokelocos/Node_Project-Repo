using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public float dayTime = 60;
    private float currentDayTime;

    [SerializeField] private Text money_text;
    [SerializeField] private Text days_text;
    [SerializeField] private Image day_image;

    private static int money;
    private static int day = 0;

    private static List<int> dayTransactions = new List<int>();

    public static int Money
    {
        get
        {
            return money;
        }
    }

    public static int Days
    {
        get
        {
            return day;
        }
    }

    public static void AddMoney(int amount)
    {
        dayTransactions.Add(amount);
        money += amount;
    }

    public void SetTimeScale(float value)
    {
        Time.timeScale = value;
    }

    public void NewDay()
    {
        foreach (var node in FindObjectsOfType<NodeView>())
        {
            AddMoney(-node.GetMantainCost());
        }

        var balance = 0;

        foreach (var transaction in dayTransactions)
        {
            balance += transaction;
        }

        //SHOW BALANCE 

        day++;
        days_text.text = day.ToString();
    }

    private void Update()
    {
        currentDayTime += Time.deltaTime;

        day_image.fillAmount = currentDayTime / dayTime;

        if (currentDayTime > dayTime)
        {
            currentDayTime = 0;

            NewDay();
        }

        if(money < 0)
        {
            money_text.color = new Color(251f/255f, 181f / 255f, 181f / 255f);
            money_text.text = "$" + (-Money);
        }
        else
        {
            money_text.color = Color.white;
            money_text.text = "$" + Money;
        }
        
    }
}