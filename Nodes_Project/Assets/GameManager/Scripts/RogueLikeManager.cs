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

    public bool TrySetRewards()
    {
        if (GameManager.Days % 3 == 0)
        {
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
            description = "Tus granjas son 30% mas lentas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Farm").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed =  1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Farm").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 0.7f;
            }
        }
    }

    public class Frost_GE : GameEffect
    {
        public Frost_GE()
        {
            title = "Helada";
            description = "Tus granjas de hortalizas son 60% mas lentas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Horticulture").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Horticulture").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 0.4f;
            }
        }
    }

    public class LazyBees_GE : GameEffect
    {
        public LazyBees_GE()
        {
            title = "Abjeas flojas";
            description = "Tus granjas de miel son 40% mas lentas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Apiculture").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Apiculture").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 0.6f;
            }
        }
    }

    public class LazyBirds_GE : GameEffect
    {
        public LazyBirds_GE()
        {
            title = "Aves flojas";
            description = "Tus granjas aves son 40% mas lentas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Aviar Farm").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Aviar Farm").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 0.6f;
            }
        }
    }

    public class BakeryBroken_GE : GameEffect
    {
        public BakeryBroken_GE()
        {
            title = "Fabrica de pan averiada";
            description = "Tus fabricas de pan son 50% mas lentas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Bakery").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Bakery").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 0.5f;
            }
        }
    }

    public class ChipsBroken_GE : GameEffect
    {
        public ChipsBroken_GE()
        {
            title = "Fabrica de frituras averiada";
            description = "Tus fabricas de fritura son 80% mas lentas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Chips Factory").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Chips Factory").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 0.2f;
            }
        }
    }

    public class JuiceBroken_GE : GameEffect
    {
        public JuiceBroken_GE()
        {
            title = "Fabrica de jugos averiada";
            description = "Tus fabricas de jugos son 20% mas lentas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Juice Factory").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Juice Factory").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 0.8f;
            }
        }
    }

    public class DairyBroken_GE : GameEffect
    {
        public DairyBroken_GE()
        {
            title = "Fabrica de lacteos averiada";
            description = "Tus fabricas de lacteos son 40% mas lentas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Dairy Factory").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Dairy Factory").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 0.6f;
            }
        }
    }

    public class ButcherRegulations_GE : GameEffect
    {
        public ButcherRegulations_GE()
        {
            title = "Regulaciones sanitarias: Carnicero";
            description = "Tus carnicerias son 70% mas lentas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Butcher").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Butcher").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 0.3f;
            }
        }
    }

    public class CakesRegulations_GE : GameEffect
    {
        public CakesRegulations_GE()
        {
            title = "Regulaciones sanitarias: Pasteleria";
            description = "Tus pastelerias son 20% mas lentas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Cake Shop").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Cake Shop").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 0.8f;
            }
        }
    }

    public class RestaurantRegulations_GE : GameEffect
    {
        public RestaurantRegulations_GE()
        {
            title = "Nueva carta";
            description = "Tus restaurantes son 50% mas lentos.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Restaurant").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Restaurant").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().speed = 0.5f;
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

    public class InventoryError_GE : GameEffect
    {
        public InventoryError_GE()
        {
            title = "Errores de inventario";
            description = "Tus supermercados tienen 25% de probabilidad de perder su inventario.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Supermarket").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().successProbability = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Supermarket").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().successProbability = 0.75f;
            }
        }
    }

    public class BurnedOil_GE : GameEffect
    {
        public BurnedOil_GE()
        {
            title = "Aceite quemado";
            description = "Tus fabricas de frituras tienen 50% de probabilidad de quemar su aceite.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Chips Factory").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().successProbability = 1;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Chips Factory").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.GetNodeData().successProbability = 0.5f;
            }
        }
    }

    public class CrazyCows_GE : GameEffect
    {
        public CrazyCows_GE()
        {
            title = "Vacas locas";
            description = "Tu ganado tienen 16% de probabilidad de morir.";
        }

        public override void RemoveEffect()
        {
            var farms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Animal Keeper").ToArray();

            foreach (var farm in farms)
            {
                farm.GetNodeData().successProbability = 1;
            }
        }

        public override void SetEffect()
        {
            var farms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Animal Keeper").ToArray();

            foreach (var farm in farms)
            {
                farm.GetNodeData().successProbability = 0.84f;
            }
        }
    }

    public class SkinnyCows_GE : GameEffect
    {
        public SkinnyCows_GE()
        {
            title = "Vacas flacas";
            description = "Tu ganado produce 72% mas lento.";
        }

        public override void RemoveEffect()
        {
            var farms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Animal Keeper").ToArray();

            foreach (var farm in farms)
            {
                farm.GetNodeData().speed = 1;
            }
        }

        public override void SetEffect()
        {
            var farms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Animal Keeper").ToArray();

            foreach (var farm in farms)
            {
                farm.GetNodeData().speed = 0.28f;
            }
        }
    }

    public class BreweringStrike_GE : GameEffect
    {
        public BreweringStrike_GE()
        {
            title = "Huelga Cervecera";
            description = "Tus fabricas de bebidas alcoholicas se encuentran cerradas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Brewering").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.isActive = true;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Brewering").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.isActive = false;
            }
        }
    }

    public class ApicultureStrike_GE : GameEffect
    {
        public ApicultureStrike_GE()
        {
            title = "Huelga Apicultora";
            description = "Tus granjas de miel se encuentran cerradas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Apiculture").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.isActive = true;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Apiculture").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.isActive = false;
            }
        }
    }

    public class PackagingStrike_GE : GameEffect
    {
        public PackagingStrike_GE()
        {
            title = "Huelga de enlatados";
            description = "Tus fabricas de enlatados se encuentran cerradas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Packaging").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.isActive = true;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Packaging").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.isActive = false;
            }
        }
    }

    public class DairyStrike_GE : GameEffect
    {
        public DairyStrike_GE()
        {
            title = "Huelga de lacteos";
            description = "Tus fabricas de lacteos se encuentran cerradas.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Dairy Factory").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.isActive = true;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Dairy Factory").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.isActive = false;
            }
        }
    }

    public class SupermarketStrike_GE : GameEffect
    {
        public SupermarketStrike_GE()
        {
            title = "Huelga de supermercados";
            description = "Tus supermercados se encuentran cerrados.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Supermarket").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.isActive = true;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Supermarket").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.isActive = false;
            }
        }
    }

    public class BistroStrike_GE : GameEffect
    {
        public BistroStrike_GE()
        {
            title = "Huelga de meseros";
            description = "Tus restaurantes elegantes se encuentran cerrados.";
        }

        public override void RemoveEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Expensive Restaurant").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.isActive = true;
            }
        }

        public override void SetEffect()
        {
            var cerealFarms = FindObjectsOfType<Production_Node>().Where(x => x.GetNodeData().name == "Expensive Restaurant").ToArray();

            foreach (var farm in cerealFarms)
            {
                farm.isActive = false;
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
