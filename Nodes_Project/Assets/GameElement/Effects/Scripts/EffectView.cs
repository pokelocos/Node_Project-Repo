using MicroFactory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectView : MonoBehaviour
{
    //private static int maxCharTitle = 35;
    //private static int maxCharDescription = 160;

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
    [SerializeField] internal Animator animator;

    public void _OnMouseEnter()
    {
        SetOpen(true);
    }

    public void _OnMouseExit()
    {
        SetOpen(false);
    }

    public void SetInfo(EffectData effect, Color c1, Color c2)
    {
        SetColors(c1, c2, c2, c2); // cambiar a 4 colores diferentes si es necesario
        SetIcon(effect.icon);
        SetText(effect.title,effect.description);
        SetTimer(effect.duration,0); 
    }

    protected void SetText(string title,string description)
    {
        this.title.text = title;
        this.description.text = description;
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

    public void SetTimer(float max, float current)
    {
        var dt = 1 - (current / max);
        mainClock.fillAmount = dt;
        timer.text = SegToString((int)(max - current));
    }

    private static string SegToString(int seg)
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
