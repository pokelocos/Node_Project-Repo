using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using MicroFactory;
using static RogueLikeManager;
using DataSystem;

public class GameManager : MonoBehaviour
{

    public int lastpoint = 0;
    public int winPoints = 5;

    [SerializeField] private Text money_text;
    [SerializeField] private Text days_text;
    [SerializeField] private Text balance_text;
    [SerializeField] private Text starAmount_text;
    [SerializeField] private GameObject pause_frame;


    public GameObject warningPanel;

    [Space]
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject winPanel;

    [Header("Balance Settings")]
    [SerializeField] private Text mantainance_text;
    [SerializeField] private Text lastEarnings_text;

    private int lastBalance = 0;
    private int lastEarnings;
    private float balance_alpha = 0; //?
    private Color balance_color = Color.green;
    private static List<int> dayTransactions = new List<int>();
    public static bool snapTool;

    public AudioSource source;
    public AudioClip winSound;

    private static int negativeDays = 0;
    private bool warningonce = true;

    //%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

    public delegate void NodeEvent(NodeController node);
    public NodeEvent OnAddNode;

    public delegate void EffectEvent(EffectController effect);
    public NodeEvent OnAddEffect;

    [Header("Managers slaves")]
    [SerializeField] private TimeManager timeManager; // hud controllers (?)
    [SerializeField] private NodeManager nodeManager;
    [SerializeField] private RogueLikeManager rogueLikeManger;

    [Header("Basics prefs")]
    [SerializeField] private NodeController node_Pref;
    [SerializeField] private EffectController effect_Pref;  
    [SerializeField] private GameObject connetion_Pref;

    [Header("Scene references")]
    [SerializeField] private Transform effectsPivot; // move this to "guiManger" (?)

    private GameState gameState;
    private List<NodeController> nodes = new List<NodeController>();
    private List<EffectController> effects = new List<EffectController>();

    #region ##Data##

    private NodeData[] nodeDatas;
    private EffectData[] effectDatas;
    private IngredientData[] ingredientsDatas;
    private Recipe[] RecipesDatas;

    #endregion

    #region ##Events##

    //public delegate void ConnectionEvent(ConnectionController connection);
    //public ConnectionEvent OnCreateConection;

    #endregion

    private void Awake()
    {
        LoadAssetsData();
        Init();

        // Set Events
        rogueLikeManger.OnSelectedReward += InstanceReward;
        timeManager.OnEndCycle += NewDay;
    }

    private void OnApplicationQuit()
    {
        gameState.Save(); //??
    }

    private void Start()
    {
        
    }

    private void Init()
    {
        var data = DataManager.LoadData<Data>();
        if (data != null)
        {
            this.gameState = data.GameState;
            LoadState(gameState);
        }
        else
        {
            Debug.LogError("[LoadData Error]: No se pudo cargar la informacion de la partida");
        }

        negativeDays = 0; // move this to gamemode conditions  -> "gamemode"
        lastBalance = gameState.Money; // move this to game conditions -> "gamemode"
        timeManager.SetTimeScale(0);
        snapTool = data.options.SnapMode;
    }

    private void LoadAssetsData()
    {
        nodeDatas = Resources.LoadAll<NodeData>("Nodes");
        Debug.Log("Load ("+nodeDatas.Length+") node resources.");
        effectDatas = Resources.LoadAll<EffectData>("Effects");
        Debug.Log("Load (" + effectDatas.Length + ") effect resources.");
        ingredientsDatas = Resources.LoadAll<IngredientData>("Materials");
        Debug.Log("Load (" + ingredientsDatas.Length + ") ingredient resources.");
    }

    public void InstanceReward(Reward reward)
    {
        
        for (int i = 0; i < reward.nodes.Length; i++)
        {
            CreateNode(reward.nodes[i],GetUnoccupiedPosition(),0);
        }

        for (int i = 0; i < reward.effects.Length; i++)
        {
           CreateEffect(reward.effects[i],0);
        }

        AddMoney(-reward.price);

        //gameManager.SetTimeScale(lastTimeScale); // esto no debiese estar aqui
    }

    

    private void LoadState(GameState gameState)
    {
        for (int i = 0; i < gameState.GetNodeAmount(); i++) 
        {
            var state = gameState.GetNode(i);
            var time = state.currentTime;
            var data = nodeDatas.First(x => x.name == state.dataName);
            CreateNode(data, state.Position, time);
        }

        for (int i = 0; i < gameState.GetEffectAmount(); i++)
        {
            var state = gameState.GetEffect(i);
            var time = state.currentTime;
            var data = effectDatas.First(x => x.name == state.effectName);
            CreateEffect(data,time);
        }

        for (int i = 0; i < gameState.GetConnectionAmount(); i++)
        {
            var state = gameState.GetConnection(i);
            var n1 = state.nodeRelationIndex.Item1;
            var n2 = state.nodeRelationIndex.Item2;
            var ingr = ingredientsDatas.First(x => x.name == state.ingredientName);
            NodeManager.ConnectNodes(nodes[n1], nodes[n2]); // falta pasarle el tipo de conexion
        }
    }

    private void CreateNode(NodeData nodeData, Vector2 pos, float startTime) 
    {
        //var pos = GetUnoccupiedPosition();
        var node = Instantiate(node_Pref, pos, Quaternion.identity);
        node.Init(nodeData,startTime);
        nodes.Add(node);
    }

    public void CreateEffect(EffectData effectData,float startTime) 
    {
        var effect = Instantiate(effect_Pref, effectsPivot);
        effect.Init(effectData,startTime);
        effects.Add(effect);
    }

    public Vector2 GetUnoccupiedPosition()
    {
        return Vector2.zero; // IMPLEMENTAR
    }


    public void ToggleSnapTool()
    {
        snapTool = !snapTool;
    }
     
    public void ToggleMoneyBalance()
    {
        GetComponent<Animator>().SetBool("Show Balance", !GetComponent<Animator>().GetBool("Show Balance"));

        int price = 0;

        foreach (var node in FindObjectsOfType<NodeController>()) // change to foreach "nodes"
        {
            price += node.GetData().maintainCost;
        }

        mantainance_text.text = "$" + price;

        lastEarnings_text.text = "$" + lastEarnings;
    }

    
    public void AddMoney(int amount) // hay que encapsular el manejo de plata o esquematizar mejor sus funciones
    {
        dayTransactions.Add(amount);
        gameState.Money += amount;

        if (gameState.Money > 0) // esto deberia estar en condiciones de ganr perder dentro de gamemode y que se asignen a eventos de esta clase
        {
            negativeDays = 0;
        }
    }
    

    public List<NodeController> GetNodesByTypes(NodeData.Type[] types) // Change parameter "NodeData.Type" to => "String" (???)
    {
        List<NodeController> toReturn = new List<NodeController>();

        foreach (var ty in types)
        {
            var temp = nodes.Where(x => x.GetData().type == ty && !toReturn.Contains(x));
            toReturn.Concat(temp);
        }

        return toReturn;
    }

    public List<NodeController> GetNodesByNames(string[] names)
    {
        List<NodeController> toReturn = new List<NodeController>();

        foreach (var name in names)
        {
            var temp = nodes.Where(x => x.GetData().name == name && !toReturn.Contains(x));
            toReturn.Concat(temp);
        }

        return toReturn;
    }


    public void NewDay()
    {
        gameState.Day++;

        if (gameState.Day % 3 == 0)
        {
            timeManager.SetTimeScale(0);
            //rogueLikeManger.OnSelectedReward = timeManager.; // last time
            var rewards = rogueLikeManger.GenerateRewards(nodeDatas, effectDatas, 3, Random.Range(2, 4), 3, gameState.Day); // nodeDatas y effectDatas podria ser estatica y global en otra clase que guarde datas
            rogueLikeManger.ShowRewards(rewards); // Esto deberia ser de algo tipo "rewardsView" y no de "rogueLikeManager"
        }

        var balance = 0;

        foreach (var transaction in dayTransactions)
        {
            balance += transaction;
        }

        lastEarnings = balance;
        balance = gameState.Money - lastBalance;
        lastBalance = gameState.Money;

        foreach (var node in nodes)
        {
            gameState.Money +=(-node.GetData().maintainCost);
        }

        dayTransactions.Clear();

        //SHOW BALANCE 
        if (balance > 0)
        {
            balance_text.text = "$" + balance;
            balance_color = Color.green;
        }
        else
        {
            balance_text.text = "$" + balance;
            balance_color = Color.red;
        }

        balance_alpha = 5;

       
        days_text.text = gameState.Day.ToString();

        if(gameState.Money < 0)
        {
            if(warningonce)
            {
                warningPanel.gameObject.SetActive(true);
                warningonce = false;
            }

            negativeDays++; // variable ql mala
        }
    }
    
    

    

    private void Update()
    {
        pause_frame.gameObject.SetActive(Time.timeScale == 0); //?

        //timeManager.TimeControlsUpdate();

        starAmount_text.text = gameState.ContractPoints + "/" + winPoints; // contract


        balance_color.a = balance_alpha;

        if (balance_alpha > 0)
            balance_alpha -= Time.deltaTime;

        balance_text.color = balance_color;

        /*
        if(negativeDays <= 0)
        {
            day_image.color = commonDay;
        }
        else if(negativeDays > 3)
        {
            day_image.color = AlertDay;
        }
        else
        {
            day_image.color = debDay;
        }
        */

        // set money value
        SetMoneyValue(gameState.Money);

        //LOSE CON
        if (negativeDays >= 3)
        {
            losePanel.SetActive(true); 
        }

        // WIN CON
        if (gameState.ContractPoints >= winPoints)
        {
            winPanel.SetActive(true);
        }

        if (lastpoint != gameState.ContractPoints)
        {
            source.PlayOneShot(winSound);
        }
        lastpoint = gameState.ContractPoints;
    }

    private void SetMoneyValue(int v)
    {
        if (v < 0)
        {
            money_text.color = new Color(251f / 255f, 181f / 255f, 181f / 255f);
            money_text.text = "-$" + (-v);
        }
        else
        {
            money_text.color = Color.white;
            money_text.text = "$" + v;
        }
    }
}