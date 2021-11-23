using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public float dayTime = 60;
    private float currentDayTime;
    
    [SerializeField] private Text money_text;
    [SerializeField] private Text days_text;
    [SerializeField] private Text balance_text;
    [SerializeField] private Image day_image;
    [SerializeField] private EffectView effectView_template;

    [Space]
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject winPanel;

    [Space, Header("HUD Buttons")]
    [SerializeField] private Toggle pauseToggle;
    [SerializeField] private Toggle playToggle;
    [SerializeField] private Toggle speedPlayToggle;

    private static int money;
    private static int day = 0;
    private int lastBalance = 0;
    private float balance_alpha = 0;
    private Color balance_color = Color.green;
    private static List<int> dayTransactions = new List<int>();



    private static Dictionary<RogueLikeManager.GameEffect, EffectView> gameEffects = new Dictionary<RogueLikeManager.GameEffect, EffectView>();

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

    public static void AddEffect(RogueLikeManager.GameEffect gameEffect)
    {
        gameEffect.SetEffect();

        var template = FindObjectOfType<GameManager>().effectView_template;
        var effectView = Instantiate(template, template.transform.parent);
        effectView.gameObject.SetActive(true);

        effectView.SetData(gameEffect.title, gameEffect.description);

        gameEffects.Add(gameEffect, effectView);
    }

    public void SetTimeScale(float value)
    {
        Time.timeScale = value;
    }

    public void NewDay()
    {
        if(FindObjectOfType<RogueLikeManager>().TrySetRewards())
        {
            SetHudToggles(true);
        }

        for (int i = gameEffects.Keys.Count - 1; i >= 0; i--)
        {
            gameEffects.Keys.ToArray()[i].daysDuration--;

            gameEffects.Keys.ToArray()[i].SetEffect();

            if (gameEffects.Keys.ToArray()[i].daysDuration <= 0)
            {
                gameEffects.Keys.ToArray()[i].RemoveEffect();

                Destroy(gameEffects.Values.ToArray()[i].gameObject);

                gameEffects.Remove(gameEffects.Keys.ToArray()[i]);
            }
        }

        foreach (var node in FindObjectsOfType<NodeView>())
        {
            AddMoney(-node.GetMantainCost());
        }

        var balance = 0;

        foreach (var transaction in dayTransactions)
        {
            balance += transaction;
        }

        balance = money - lastBalance;
        lastBalance = money;

        //SHOW BALANCE 
        if (balance > 0)
        {
            balance_text.text = "$" + balance;
            balance_color = Color.green;
        }
        else
        {
            balance_text.text = "-$" + balance;
            balance_color = Color.red;
        }

        balance_alpha = 5;

        day++;
        days_text.text = day.ToString();
    }
    
    public void SetHudToggles(bool active)
    {
        pauseToggle.GetComponentInChildren<Image>().color = active ? Color.red : Color.white;
        pauseToggle.interactable = !active;
        playToggle.interactable = !active;
        speedPlayToggle.interactable = !active;
    }

    private void Update()
    {
        if (pauseToggle.interactable == false && Time.timeScale == 1)
            SetHudToggles(false);

        currentDayTime += Time.deltaTime;

        day_image.fillAmount = currentDayTime / dayTime;

        balance_color.a = balance_alpha;

        if (balance_alpha > 0)
            balance_alpha -= Time.deltaTime;

        balance_text.color = balance_color;

        if (currentDayTime > dayTime)
        {
            currentDayTime = 0;

            NewDay();
        }

        // set money value
        SetMoneyValue(Money);

        //LOSE CON
        if (money < -150)
        {
            losePanel.SetActive(true);
        }

        // WIN CON
        if (day > 15)
        {
            winPanel.SetActive(true);
        }
    }

    private void SetMoneyValue(int v)
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