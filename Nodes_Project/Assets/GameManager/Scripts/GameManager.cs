using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public float dayTime = 60;
    private float currentDayTime;
    public static int points = 0;
    public int lastpoint = 0;
    public int winPoints = 5;
    
    [SerializeField] private Text money_text;
    [SerializeField] private Text days_text;
    [SerializeField] private Text balance_text;
    [SerializeField] private Text starAmount_text;
    [SerializeField] private Image day_image;
    [SerializeField] private EffectView effectView_template;

    [Space]
    public Color commonDay;
    public Color debDay;
    public Color AlertDay;

    public GameObject warningPanel;

    [Space]
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject winPanel;

    [Space, Header("HUD Buttons")]
    [SerializeField] private Toggle pauseToggle;
    [SerializeField] private Toggle playToggle;
    [SerializeField] private Toggle speedPlayToggle;

    [SerializeField] private static int money = 1000;
    private static int day = 0;
    private int lastBalance = 0;
    private float balance_alpha = 0;
    private Color balance_color = Color.green;
    private static List<int> dayTransactions = new List<int>();

    private static Dictionary<RogueLikeManager.GameEffect, EffectView> gameEffects = new Dictionary<RogueLikeManager.GameEffect, EffectView>();

    public AudioSource source;
    public AudioClip winSound;

    private int negativeDays = 0;
    private bool warningonce = true;

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
            balance_text.text = "$" + balance;
            balance_color = Color.red;
        }

        balance_alpha = 5;

        day++;
        days_text.text = day.ToString();

        if(money < 0)
        {
            if(warningonce)
            {
                warningPanel.gameObject.SetActive(true);
                warningonce = false;
            }

            negativeDays++; // variable ql mala
        }
        else
        {
            negativeDays = 0;
        }
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

        starAmount_text.text = points + "/" + winPoints;

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

        if(negativeDays <= 0)
        {
            day_image.color = commonDay;
        }
        else if(negativeDays >= 3)
        {
            day_image.color = AlertDay;
        }
        else
        {
            day_image.color = debDay;
        }

        // set money value
        SetMoneyValue(Money);

        //LOSE CON
        if (negativeDays >= 3)
        {
            losePanel.SetActive(true); 
        }

        // WIN CON
        if (points >= winPoints)
        {
            winPanel.SetActive(true);
        }

        if (lastpoint != points)
        {
            source.PlayOneShot(winSound);
        }
        lastpoint = points;
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