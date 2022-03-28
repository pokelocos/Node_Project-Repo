using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System;
using MicroFactory;


public class RogueLikeManager : MonoBehaviour // reward manager
{
    [SerializeField] private GameObject rewards_panel;
    [SerializeField] private GameObject rewards_menu;

    private Reward[] rewards = new Reward[3];

    [SerializeField] private RewardView[] rewardsViews;
    float lastTimeScale = 1;

    public delegate void RewardsEvent(Reward r);
    public RewardsEvent OnSelectedReward;

    private void Start()
    {
        rewards_panel.SetActive(false);
    }

    public Reward GenerateReward(NodeData[] bundleNodeData,EffectData[] bundleEffectData,int nodeAmount,int effectAmount, int difficulty)
    {
        var nodes = new NodeData[nodeAmount];
        var effects = new EffectData[effectAmount];

        for (int i = 0; i < nodeAmount; i++)
        {
            nodes[i] = bundleNodeData[UnityEngine.Random.Range(0, bundleNodeData.Length)];
        }
        for (int i = 0; i < effectAmount; i++)
        {
            effects[i] = bundleEffectData[UnityEngine.Random.Range(0, bundleEffectData.Length)];
        }
        return new Reward(nodes,effects,difficulty);
    }

    public Reward[] GenerateRewards(NodeData[] bundleNodeData, EffectData[] bundleEffectData, int nodeAmount, int effectAmount,int rewardAmount, int difficulty)
    {
        var rewards = new Reward[rewardAmount];

        for (int i = 0; i < rewardAmount; i++)
        {
            rewards[i] = GenerateReward(bundleNodeData,bundleEffectData,nodeAmount,effectAmount,difficulty);
        }
        return rewards;
    }

    public void ShowRewards(Reward[] rewards) // esto deberia estar en la view del panel de estra
    {
        lastTimeScale = Time.timeScale;
        rewards_panel.SetActive(true);

        for (int i = 0; i < rewards.Length; i++)
        {
            //var day = 11;// change tis number using "State.day"
            //var difficulty = day;
            //rewards[i] = new Reward(nodeDatas.ToList(), effectDatas.ToList(), difficulty);
            //if (day == 0)
            //    rewards[i].price = 0;
            
            rewardsViews[i].ShowReward(rewards[i]);
        }
    }

    public void SelectReward(int index)
    {
        OnSelectedReward?.Invoke(rewards[index]);
        rewards_panel.SetActive(false);
    }

   

    public NodeData[] BalanceNodeByDays(NodeData[] nodesPool,int day)
    {
        List<NodeData> allNodes = new List<NodeData>(nodesPool);

        if (day <= 12)
        {
            if (day <= 3)
            {
                for (int i = allNodes.Count - 1; i >= 0; i--)
                {
                    if (!(allNodes[i].categorie is NodeData.Categorie.PRODUCTOR))
                    {
                        if (UnityEngine.Random.Range(0, 1f) > 0.5f)
                        {
                            allNodes.RemoveAt(i);
                        }
                    }
                }
            }

            if (day > 3 && day <= 9)
            {
                for (int i = allNodes.Count - 1; i >= 0; i--)
                {
                    if (!(allNodes[i].categorie is NodeData.Categorie.MANUFACTORER))
                    {
                        if (UnityEngine.Random.Range(0, 1f) > 0.5f)
                        {
                            allNodes.RemoveAt(i);
                        }
                    }
                }
            }

            if (day > 9 && day <= 12)
            {
                for (int i = allNodes.Count - 1; i >= 0; i--)
                {
                    if (!(allNodes[i].categorie is NodeData.Categorie.SHOP))
                    {
                        if (UnityEngine.Random.Range(0, 1f) > 0.5f)
                        {
                            allNodes.RemoveAt(i);
                        }
                    }
                }
            }
        }

        return allNodes.ToArray();
    }

    public static IEnumerable<System.Type> FindDerivedTypes(Assembly assembly, System.Type baseType)
    {
        return assembly.GetTypes().Where(t => t != baseType && baseType.IsAssignableFrom(t));
    }

    public void ChangeRewardsVisibility()
    {
        rewards_menu.SetActive(!rewards_menu.activeSelf);
    }

    public bool RewardPanelActive()
    {
        return rewards_panel.activeSelf;
    }
}
