using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System;
using MicroFactory;


public class RewardManager : MonoBehaviour // reward manager
{
    private Reward[] rewards = new Reward[3];

    [SerializeField] private RewardView[] rewardsViews;
    float lastTimeScale = 1;

    public delegate void RewardsEvent(Reward r);
    public RewardsEvent OnSelectedReward;

    private void Start()
    {
        this.gameObject.SetActive(false);
    }

    public Reward GenerateReward(NodeData[] bundleNodeData,EffectData[] bundleEffectData,int nodeAmount,int effectAmount, int difficulty)
    {
        var nodes = new NodeData[nodeAmount];
        var effects = new EffectData[effectAmount];

        for (int i = 0; i < nodeAmount; i++)
        {
            var posibles = DifficultyPoolAdjust(bundleNodeData,difficulty);
            nodes[i] = posibles[UnityEngine.Random.Range(0, posibles.Length)];
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
        this.rewards = rewards;
        
        for (int i = 0; i < rewards.Length; i++)
        {
            rewardsViews[i].SetInformation(rewards[i]);
        }
        this.gameObject.SetActive(true);
    }

    public void SelectReward(int index)
    {
        Debug.Log(this.rewards[index]);
        OnSelectedReward?.Invoke(this.rewards[index]);
        this.gameObject.SetActive(false);
    }

   
    /// <summary>
    /// Recive un grupo de nodos, elimina nodos dependiendo de su tipo para ajustar
    /// Los reard al momento de la partida
    /// </summary>
    /// <param name="nodesPool"></param>
    /// <param name="actualCicle"></param>
    /// <returns></returns>
    public NodeData[] DifficultyPoolAdjust(NodeData[] nodesPool,int actualCicle)
    {
        List<NodeData> allNodes = new List<NodeData>(nodesPool);

        if (actualCicle <= 3)
        {
            return nodesPool.Where(n => n.categorie is NodeData.Categorie.PRODUCTOR || UnityEngine.Random.Range(0, 1f) > 0.5f).ToArray();
        }
        else if (actualCicle <= 9)
        {
            return nodesPool.Where(n => n.categorie is NodeData.Categorie.MANUFACTORER || UnityEngine.Random.Range(0, 1f) > 0.5f).ToArray();
        }
        else if (actualCicle <= 12)
        {
            return nodesPool.Where(n => n.categorie is NodeData.Categorie.SHOP || UnityEngine.Random.Range(0, 1f) > 0.5f).ToArray();
        }

        return allNodes.ToArray();
    }

    public static IEnumerable<System.Type> FindDerivedTypes(Assembly assembly, System.Type baseType)
    {
        return assembly.GetTypes().Where(t => t != baseType && baseType.IsAssignableFrom(t));
    }
}
