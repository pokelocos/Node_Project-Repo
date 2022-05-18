using MicroFactory;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WarningTest : MonoBehaviour
{
    private List<EffectData> effects;
    //private EffectViewManager manager;

    public void Start()
    {
        effects = Resources.LoadAll<EffectData>("Effects/").ToList();
       // manager = FindObjectOfType<EffectViewManager>();
    }

    public void AddRandomEffect()
    {
        //manager.AddEffect(effects[Random.Range(0,effects.Count)]);
    }
}
