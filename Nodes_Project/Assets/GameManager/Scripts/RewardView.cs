using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardView : MonoBehaviour
{
    [SerializeField] private Image[] nodes_body;
    [SerializeField] private Image[] nodes_icon;
    [SerializeField] private Text price;
    [SerializeField] private Text effects;

    public void ShowReward(RogueLikeManager.Reward reward)
    {
        for (int i = 0; i < reward.nodes.Length; i++)
        {
            nodes_body[i].color = reward.nodes[i].GetColor();
            nodes_icon[i].sprite = reward.nodes[i].GetNodeData().icon;
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
            effects.text += reward.effects[i].description + "\n";
        }
    }
}
