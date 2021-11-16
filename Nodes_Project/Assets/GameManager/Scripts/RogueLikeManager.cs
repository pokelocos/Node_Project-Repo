using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RogueLikeManager : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private GameObject rewards_menu;

    private Reward[] rewards = new Reward[3];

    [SerializeField] private RewardView[] rewardsViews;

    private void Start()
    {
        rewards_menu.SetActive(false);
        gameManager = FindObjectOfType<GameManager>();

        new Reward();
    }

    private void Update()
    {
        if (rewards_menu.activeSelf)
            gameManager.SetTimeScale(0);
    }

    public void TrySetRewards()
    {
        if (GameManager.Days % 3 == 0)
        {
            rewards_menu.SetActive(true);

            for (int i = 0; i < rewards.Length; i++)
            {
                rewards[i] = new Reward();
                rewardsViews[i].ShowReward(rewards[i]);
            }
        }
    }

    public void SelectReward(int index)
    {
        for (int i = 0; i < rewards[index].nodes.Length; i++)
        {
            Instantiate(rewards[index].nodes[i], null);
        }

        GameManager.AddMoney(-rewards[index].price);

        gameManager.SetTimeScale(1);
        rewards_menu.SetActive(false);
    }

    public class Reward
    {
        public NodeView[] nodes = new NodeView[3];
        public GameEffect[] effects;
        public int price;

        public Reward()
        {
            var allNodes = Resources.LoadAll<NodeView>("Node Prefabs");

            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i] = allNodes[Random.Range(0, allNodes.Length - 1)];

                price += nodes[i].GetMantainCost();
            }

            int effectsCount = 0;

            if (Random.Range(0, 1f) > 0.5f)
            {
                effectsCount++;

                if (Random.Range(0, 1f) > 0.75f)
                    effectsCount++;
            }

            price /= effectsCount + 1;

            price *= Random.Range(1, 3) * GameManager.Days;

            effects = new GameEffect[effectsCount];

            for (int i = 0; i < effectsCount; i++)
            {
                effects[i] = new GameEffect();
            }
        }
    }

    public class GameEffect
    {
        public string description = "Effect";
    }
}
