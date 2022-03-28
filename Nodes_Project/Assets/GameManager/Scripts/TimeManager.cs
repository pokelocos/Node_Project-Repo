using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private Toggle pauseToggle;
    [SerializeField] private Toggle playToggle;
    [SerializeField] private Toggle speedPlayToggle;

    [Space, Header("audio")]
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip pause_audio;
    [SerializeField] private AudioClip play_audio;
    [SerializeField] private AudioClip speed_audio;

    [Space]
    public Color commonDay;
    public Color debDay;
    public Color AlertDay;

    [SerializeField] private Image day_image;

    public float dayTime = 60;
    private float currentDayTime;
    //private float currentCicle = 0;

    public delegate void TimeEvent();
    public TimeEvent OnEndCycle;


    public void Update()
    {

        TimeControlsUpdate();

        currentDayTime += Time.deltaTime;
        day_image.fillAmount = currentDayTime / dayTime;

        if (currentDayTime > dayTime)
        {
            OnEndCycle?.Invoke();
            currentDayTime = 0;
            
            //NewDay();
        }
    }


    public void SetTimeScale(float value)
    {
        var options = DataSystem.StaticData.Data.options;
        var v = options.generalVolumen * options.effectVolumen;

        if (value == Time.timeScale)
            return;

        Time.timeScale = value;

        if (Time.timeScale == 0)
        {
            source.PlayOneShot(pause_audio, v);
        }
        else if (Time.timeScale == 1)
        {
            source.PlayOneShot(play_audio, v);
        }
        else
        {
            source.PlayOneShot(speed_audio, v );
        }
    }

    public void TimeControlsUpdate()
    {
        if (pauseToggle.interactable == false && Time.timeScale == 1)
            SetHudToggles(false);

        if (Time.timeScale == 0)
        {
            pauseToggle.isOn = true;

        }
        else if (Time.timeScale == 1)
        {
            playToggle.isOn = true;
        }
        else
        {
            speedPlayToggle.isOn = true;
        }
    }

    public void SetHudToggles(bool active)
    {
        pauseToggle.GetComponentInChildren<Image>().color = active ? Color.red : Color.white;
        pauseToggle.interactable = !active;
        playToggle.interactable = !active;
        speedPlayToggle.interactable = !active;
    }
}