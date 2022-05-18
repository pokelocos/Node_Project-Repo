using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallAudioSource : MonoBehaviour
{
    public void CallSound(AudioClip clip)
    {
        SecondRadio.PlaySFX(clip);
    }
}
