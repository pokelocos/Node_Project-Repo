using MicroFactory.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static RogueLikeManager;

public class EffectView : MonoBehaviour
{
    //private static int maxCharTitle = 35;
    //private static int maxCharDescription = 160;

    public delegate void EffectEvent(GameEffect effect);
    public EffectEvent OnEndEffect;

    private GameEffect effect;

    [SerializeField] private Image closedTab;
    [SerializeField] private Image openTab;
    [SerializeField] private Image background;
    [SerializeField] private Image labelTab;

    [SerializeField] private Text title;
    [SerializeField] private Text description;
    [SerializeField] private Text timer;

    [SerializeField] private Image mainIcon;
    [SerializeField] private Image secondaryIcon;

    [SerializeField] private Image mainClock;
    [SerializeField] private Animator animator;

    //timer
    private float maxTime = 1; // no se si esta clase deba encargarse de timer
    private float currentTime = 0;

    // Update is called once per frame
    void Update()
    {
        if (effect == null)
            return;

        //timer
        if (currentTime >= maxTime)
        {
            OnEndEffect?.Invoke(effect);
            animator.SetTrigger("close");
            animator.SetTrigger("finish");
        }
        currentTime += Time.deltaTime;
        SetTimer(maxTime, currentTime);
    }

    public void _OnMouseEnter()
    {
        SetOpen(true);
    }

    public void _OnMouseExit()
    {
        SetOpen(false);
    }

    public void SetInfo(GameEffect effect, Color c1, Color c2)
    {
        this.effect = effect;
        SetColors(c1, c2, c2, c2); // cambiar a 4 colores diferentes si es necesario
        SetIcon(effect.icon);
        SetText(effect.title,effect.description);
        SetTimer(effect.duration,0); 
    }

    protected void SetText(string title,string description)
    {
        this.title.text = effect.title;
        this.description.text = effect.description;
    }

    protected void SetOpen(bool b)
    {
        if(b)
            animator.SetTrigger("open");
        else
            animator.SetTrigger("close");

        openTab.gameObject.SetActive(b);
        closedTab.gameObject.SetActive(!b);
    }

    private void Destroy()
    {
        Destroy(this.gameObject);
    }

    protected void SetIcon(Sprite sprite)
    {
        mainIcon.sprite = sprite;
        secondaryIcon.sprite = sprite;
    }

    protected void SetColors(Color background,Color text,Color icon,Color clock)
    {
        closedTab.color = background;
        openTab.color = background;
        this.background.color = background;
        labelTab.color = background;

        title.color = text;
        description.color = text;

        timer.color = icon;
        mainIcon.color = icon;
        secondaryIcon.color = icon;

        mainClock.color = clock;
    }

    protected void SetTimer(float max, float current)
    {
        maxTime = max;
        currentTime = current;
        var dt = 1 - (currentTime/ maxTime);
        mainClock.fillAmount = dt;
        timer.text = SegToString((int)(max - current));
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
