using MicroFactory.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static RogueLikeManager;

public class EffectView : MonoBehaviour
{
    private int maxCharTitle = 35;
    private int maxCharDescription = 160;

    private GameEffect effect;

    [SerializeField] private Image closedBckground;
    [SerializeField] private Image openBckground;

    [SerializeField] private Text title;
    [SerializeField] private Text description;
    [SerializeField] private Text timer;

    [SerializeField] private Image mainIcon;
    [SerializeField] private Image secondaryIcon;

    [SerializeField] private Image mainClock;
    [SerializeField] private Image secondaryClock;

    //timer
    private float maxTime = 1; // no se si esta clase deba encargarse de timer
    private float currentTime = 0;

    // Update is called once per frame
    void Update()
    {
        if (effect == null)
            return;

        /////// esta implementacion es temporal ///////
        if (currentTime >= maxTime)
        {
            Destroy(this.gameObject);
        }
        SetTimer(maxTime, currentTime);
        /////// esta implementacion es temporal ///////
    }

    private void OnMouseEnter()
    {
        SetOpen(true);
    }

    private void OnMouseExit()
    {
        SetOpen(false);
    }

    public void SetInfo(GameEffect effect, Color c1, Color c2)
    {
        this.effect = effect;
        SetColors(c1, c2, c2, c2); // cambiar a 4 colores diferentes si es necesario
        SetIcon(effect.icon);
        SetText(effect.title,effect.description);
        SetTimer(effect.duration * 60, effect.duration * 60); // este 60 es para evitar cancersito, pero trae un cancersito diferente
    }

    protected void SetText(string title,string description)
    {
        this.title.text = effect.title;
        this.description.text = effect.description;
    }

    protected void SetOpen(bool b)
    {
        openBckground.gameObject.SetActive(b);
        closedBckground.gameObject.SetActive(!b);
    }

    protected void SetIcon(Sprite sprite)
    {
        mainIcon.sprite = sprite;
        secondaryIcon.sprite = sprite;
    }

    protected void SetColors(Color background,Color text,Color icon,Color clock)
    {
        closedBckground.color = background;
        openBckground.color = background;

        title.color = text;
        description.color = text;

        timer.color = icon;
        mainIcon.color = icon;
        secondaryIcon.color = icon;

        mainClock.color = clock;
        secondaryClock.color = clock;
    }

    protected void SetTimer(float max, float current)
    {
        maxTime = effect.duration * 60; // este 60 es para evitar cancersito, pero trae un cancersito diferente
        currentTime = effect.duration * 60;
        var dt = maxTime / currentTime;
        mainClock.fillAmount = dt;
        secondaryClock.fillAmount = dt;
        timer.text = SegToString((int)current);
    }

    public static string SegToString(int seg)
    {
        var m = (seg / 60).ToString();
        var s = (seg % 60).ToString();

        if (m.Length <= 1)
            m = 0 + m;

        if (s.Length <= 1)
            s = 0 + s;

        return m + ":" + s;
    }
}
