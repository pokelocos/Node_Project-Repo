using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//[Obsolete("esto se esta usando ??", true)]
public class RewardView : MonoBehaviour
{
    [SerializeField] private Image[] nodes_body; // replace  -> "NodeView"
    [SerializeField] private Image[] nodes_icon; 
    [SerializeField] private Text price;
    [SerializeField] private Text effects; // replace  -> "EffectView"

    public void ShowReward(Reward reward)
    {
        for (int i = 0; i < reward.nodes.Length; i++)
        {
            nodes_body[i].color = reward.nodes[i].color;
            nodes_icon[i].sprite = reward.nodes[i].icon;
        }

        if (reward.price > 0)
        {
            price.text = "$" + reward.price;
        }
        else
        {
            price.text = "FREE";
        }

        effects.text = string.Empty;

        for (int i = 0; i < reward.effects.Length; i++)
        {
            effects.text += "*" + reward.effects[i].title + "\n";
        }
    }
}
