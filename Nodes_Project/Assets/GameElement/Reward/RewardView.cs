using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardView : MonoBehaviour
{
    // pref reference
    [SerializeField] private SimpleGuiView[] nodes = new SimpleGuiView[3];
    [SerializeField] private SimpleGuiView[] effects = new SimpleGuiView[3];
    [SerializeField] private Text price;

    public void SetInformation(Reward reward)
    {
        price.text = "$" + reward.price;

        for (int i = 0; i < nodes.Length; i++)
        {
            if(i < reward.nodes.Length)
            {
                nodes[i].gameObject.SetActive(true);
                var node = reward.nodes[i];
                nodes[i].SetInfo(node.icon,node.color);
            }
            else
            {
                nodes[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < effects.Length; i++)
        {
            if (i < reward.effects.Length)
            {
                effects[i].gameObject.SetActive(true);
                var effect = reward.effects[i];
                effects[i].SetInfo(effect.icon, effect.color);
            }
            else
            {
                effects[i].gameObject.SetActive(false);
            }
        }
    }

}
