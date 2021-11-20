using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;

public class RogueLikeManager : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private GameObject rewards_panel;
    [SerializeField] private GameObject rewards_menu;

    private Reward[] rewards = new Reward[3];

    [SerializeField] private RewardView[] rewardsViews;

    private void Start()
    {
        rewards_panel.SetActive(false);
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (rewards_panel.activeSelf)
            gameManager.SetTimeScale(0);
    }

    public void TrySetRewards()
    {
        if (GameManager.Days % 3 == 0)
        {
            rewards_panel.SetActive(true);

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

        for (int i = 0; i < rewards[index].effects.Length; i++)
        {
            GameManager.AddEffect(rewards[index].effects[i]);
        }

        GameManager.AddMoney(-rewards[index].price);

        gameManager.SetTimeScale(1);
        rewards_panel.SetActive(false);


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
                nodes[i] = allNodes[Random.Range(0, allNodes.Length)];

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
                var effectsClasses = FindDerivedTypes(Assembly.GetExecutingAssembly(), typeof(GameEffect)).ToArray();

                effects[i] = System.Activator.CreateInstance(effectsClasses[Random.Range(0, effectsClasses.Length)]) as GameEffect;

                effects[i].daysDuration = Random.Range(1, 3);
            }
        }
    }

    public abstract class GameEffect
    {
        public string title = "Effect";
        public string description = "Effect";
        public int daysDuration = 1;

        protected GameEffect()
        {
            
        }

        public abstract void SetEffect();

        public abstract void RemoveEffect();
    }

    public class Drought_GE : GameEffect
    {
        public Drought_GE()
        {
            title = "Sequia";
            description = "Tus granjas son 30% mas lenas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Farm").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed +=  1 * 0.3f;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Farm").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed *= 0.7f;
            }
        }
    }

    public class Plague_GE : GameEffect
    {
        public Plague_GE()
        {
            title = "Plaga";
            description = "Tus granjas tienen 35% de probabilidad de perder la cosecha.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Farm").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().successProbability = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Farm").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().successProbability = 0.65f;
            }
        }
    }

    public static IEnumerable<System.Type> FindDerivedTypes(Assembly assembly, System.Type baseType)
    {
        return assembly.GetTypes().Where(t => t != baseType && baseType.IsAssignableFrom(t));
    }

    public void ChangeRewardsVisibility()
    {
        rewards_menu.SetActive(!rewards_menu.activeSelf);
    }
}
