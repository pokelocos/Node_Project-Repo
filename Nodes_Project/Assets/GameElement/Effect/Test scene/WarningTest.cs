using MicroFactory.Effects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WarningTest : MonoBehaviour
{
    private List<GameEffect> effects;
    private EffectManager manager;

    public void Start()
    {
        effects = Resources.LoadAll<GameEffect>("Effects/").ToList();
        manager = FindObjectOfType<EffectManager>();
    }

    public void AddRandomEffect()
    {
        manager.AddEffect(effects[Random.Range(0,effects.Count)]);
    }
}
