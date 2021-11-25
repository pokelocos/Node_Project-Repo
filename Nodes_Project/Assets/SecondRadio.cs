using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondRadio : MonoBehaviour
{
    public static SecondRadio instance;

    public AudioSource source;
    public float volPitch;

    public static SecondRadio pref;

    private void Awake()
    {
        volPitch = source.pitch;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static void PlaySFX(AudioClip clip)
    {
        var source = SecondRadio.instance.source;
        source.pitch = instance.volPitch + Random.Range(-0.1f,0.1f);
        source.PlayOneShot(clip);
    }
}
