using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System;

[Obsolete]
public class RogueLikeManager : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private GameObject rewards_panel;
    [SerializeField] private GameObject rewards_menu;

    private Reward[] rewards = new Reward[3];

    [SerializeField] private RewardView[] rewardsViews;
    float lastTimeScale = 1;

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

    public bool TrySetRewards()
    {
        if (GameManager.Days % 3 == 0)
        {
            lastTimeScale = Time.timeScale;

            rewards_panel.SetActive(true);

            for (int i = 0; i < rewards.Length; i++)
            {
                rewards[i] = new Reward();
                rewardsViews[i].ShowReward(rewards[i]);
            }

            return true;
        }
        return false;
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

        rewards_panel.SetActive(false);

        gameManager.SetTimeScale(lastTimeScale);
    }

    public class Reward
    {
        public NodeView[] nodes = new NodeView[3];
        public GameEffect_OB[] effects;
        public int price;

        public Reward()
        {
            var allNodes = Resources.LoadAll<NodeView>("Node Prefabs");

            allNodes = BalanceNodeByDays(allNodes);

            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i] = allNodes[UnityEngine.Random.Range(0, allNodes.Length)];

                price += nodes[i].GetMantainCost();
            }

            int effectsCount = 0;

            if (UnityEngine.Random.Range(0, 1f) > 0.5f)
            {
                effectsCount++;

                if (UnityEngine.Random.Range(0, 1f) > 0.75f)
                    effectsCount++;
            }

            price /= effectsCount + 1;

            price += UnityEngine.Random.Range(1, 3) * GameManager.Days * 5;

            effects = new GameEffect_OB[effectsCount];

            if (GameManager.Days == 0)
                price = 0;

            for (int i = 0; i < effectsCount; i++)
            {
                var effectsClasses = FindDerivedTypes(Assembly.GetExecutingAssembly(), typeof(GameEffect_OB)).ToArray();

                effects[i] = System.Activator.CreateInstance(effectsClasses[UnityEngine.Random.Range(0, effectsClasses.Length)]) as GameEffect_OB;

                effects[i].daysDuration = UnityEngine.Random.Range(3, 6);
                effects[i].duration = effects[i].daysDuration;
            }
        }

        public NodeView[] BalanceNodeByDays(NodeView[] nodesPool)
        {
            List<NodeView> allNodes = new List<NodeView>(nodesPool);

            if (GameManager.Days <= 12)
            {
                if (GameManager.Days <= 3)
                {
                    for (int i = allNodes.Count - 1; i >= 0; i--)
                    {
                        if (!(allNodes[i].GetNodeData().categorie is NodeData.Categorie.PRODUCTOR))
                        {
                            if (UnityEngine.Random.Range(0, 1f) > 0.5f)
                            {
                                allNodes.RemoveAt(i);
                            }
                        }
                    }
                }

                if (GameManager.Days > 3 && GameManager.Days <= 9)
                {
                    for (int i = allNodes.Count - 1; i >= 0; i--)
                    {
                        if (!(allNodes[i].GetNodeData().categorie is NodeData.Categorie.MANUFACTORER))
                        {
                            if (UnityEngine.Random.Range(0, 1f) > 0.5f)
                            {
                                allNodes.RemoveAt(i);
                            }
                        }
                    }
                }

                if (GameManager.Days > 9 && GameManager.Days <= 12)
                {
                    for (int i = allNodes.Count - 1; i >= 0; i--)
                    {
                        if (!(allNodes[i].GetNodeData().categorie is NodeData.Categorie.SHOP))
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
    }

    public abstract class GameEffect_OB
    {
        public string title = "Effect";
        public string description = "Effect";
        public Sprite icon = null;
        public int daysDuration = 1;
        public int duration = 1;
        protected GameEffect_OB()
        {
            
        }

        public abstract void SetEffect();

        public abstract void RemoveEffect();
    }

    public class Drought_GE : GameEffect_OB
    {
        public Drought_GE()
        {
            title = "Sequia";
            description = "Tus granjas son 30% mas lentas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().type == NodeData.Type.FARM || x.GetNodeData().type == NodeData.Type.PLANTATION || x.GetNodeData().type == NodeData.Type.FIELD).ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed =  1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().type == NodeData.Type.FARM || x.GetNodeData().type == NodeData.Type.PLANTATION || x.GetNodeData().type == NodeData.Type.FIELD).ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 0.7f;
            }
        }
    }

    public class Frost_GE : GameEffect_OB
    {
        public Frost_GE()
        {
            title = "Helada";
            description = "Tus granjas de hortalizas son 60% mas lentas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Horticulture").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Horticulture").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 0.4f;
            }
        }
    }

    public class LazyBees_GE : GameEffect_OB
    {
        public LazyBees_GE()
        {
            title = "Abjeas flojas";
            description = "Tus granjas de miel son 40% mas lentas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Apiculture").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Apiculture").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 0.6f;
            }
        }
    }

    public class LazyBirds_GE : GameEffect_OB
    {
        public LazyBirds_GE()
        {
            title = "Aves flojas";
            description = "Tus granjas aves son 40% mas lentas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Aviar Farm").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Aviar Farm").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 0.6f;
            }
        }
    }

    public class BakeryBroken_GE : GameEffect_OB
    {
        public BakeryBroken_GE()
        {
            title = "Fabrica de pan averiada";
            description = "Tus fabricas de pan son 50% mas lentas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Bakery").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Bakery").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 0.5f;
            }
        }
    }

    public class ChipsBroken_GE : GameEffect_OB
    {
        public ChipsBroken_GE()
        {
            title = "Fabrica de frituras averiada";
            description = "Tus fabricas de fritura son 80% mas lentas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Chips Factory").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Chips Factory").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 0.2f;
            }
        }
    }

    public class JuiceBroken_GE : GameEffect_OB
    {
        public JuiceBroken_GE()
        {
            title = "Fabrica de jugos averiada";
            description = "Tus fabricas de jugos son 20% mas lentas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Juice Factory").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Juice Factory").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 0.8f;
            }
        }
    }

    public class DairyBroken_GE : GameEffect_OB
    {
        public DairyBroken_GE()
        {
            title = "Fabrica de lacteos averiada";
            description = "Tus fabricas de lacteos son 40% mas lentas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Dairy Factory").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Dairy Factory").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 0.6f;
            }
        }
    }

    public class ButcherRegulations_GE : GameEffect_OB
    {
        public ButcherRegulations_GE()
        {
            title = "Regulaciones sanitarias: Carnicero";
            description = "Tus carnicerias son 70% mas lentas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Butcher").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Butcher").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 0.3f;
            }
        }
    }

    public class CakesRegulations_GE : GameEffect_OB
    {
        public CakesRegulations_GE()
        {
            title = "Regulaciones sanitarias: Pasteleria";
            description = "Tus pastelerias son 20% mas lentas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Cake Shop").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Cake Shop").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 0.8f;
            }
        }
    }

    public class RestaurantRegulations_GE : GameEffect_OB
    {
        public RestaurantRegulations_GE()
        {
            title = "Nueva carta";
            description = "Tus restaurantes son 50% mas lentos.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Restaurant").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Restaurant").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 0.5f;
            }
        }
    }

    public class Plague_GE : GameEffect_OB
    {
        public Plague_GE()
        {
            title = "Plaga";
            description = "Tus granjas tienen 35% de probabilidad de perder la cosecha.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Farm").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().successProbability = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Farm").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().successProbability = 0.65f;
            }
        }
    }

    public class InventoryError_GE : GameEffect_OB
    {
        public InventoryError_GE()
        {
            title = "Errores de inventario";
            description = "Tus supermercados tienen 25% de probabilidad de perder su inventario.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Supermarket").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().successProbability = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Supermarket").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().successProbability = 0.75f;
            }
        }
    }

    public class BurnedOil_GE : GameEffect_OB
    {
        public BurnedOil_GE()
        {
            title = "Aceite quemado";
            description = "Tus fabricas de frituras tienen 50% de probabilidad de quemar su aceite.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Chips Factory").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().successProbability = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Chips Factory").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().successProbability = 0.5f;
            }
        }
    }

    public class CrazyCows_GE : GameEffect_OB
    {
        public CrazyCows_GE()
        {
            title = "Vacas locas";
            description = "Tu ganado tienen 16% de probabilidad de morir.";
        }

        public override void RemoveEffect()
        {
            var farms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Animal Keeper").ToArray();

            foreach (var farm in farms)
            {
                farm.GetNodeData().successProbability = 1;
            }
        }

        public override void SetEffect()
        {
            var farms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Animal Keeper").ToArray();

            foreach (var farm in farms)
            {
                farm.GetNodeData().successProbability = 0.84f;
            }
        }
    }

    public class SkinnyCows_GE : GameEffect_OB
    {
        public SkinnyCows_GE()
        {
            title = "Vacas flacas";
            description = "Tu ganado produce 72% mas lento.";
        }

        public override void RemoveEffect()
        {
            var farms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Animal Keeper").ToArray();

            foreach (var farm in farms)
            {
                farm.GetNodeData().speed = 1;
            }
        }

        public override void SetEffect()
        {
            var farms = FindObjectsOfType<NodeView>().Where(x => x.GetNodeData().name == "Animal Keeper").ToArray();

            foreach (var farm in farms)
            {
                farm.GetNodeData().speed = 0.28f;
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

    public bool RewardPanelActive()
    {
        return rewards_panel.activeSelf;
    }
}
