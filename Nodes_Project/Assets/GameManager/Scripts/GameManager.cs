using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using MicroFactory;
using static RewardManager;
using DataSystem;
using UnityEditor;
using System;

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
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private NodeManager nodeManager; 
    [SerializeField] private RewardManager rewardManger;

    [Header("Basics prefs")]
    [SerializeField] private NodeController node_Pref;
    [SerializeField] private EffectController effect_Pref;  
    [SerializeField] private ConnectionController connetion_Pref;

    [Header("Scene references")]
    [SerializeField] private Transform effectsPivot; // move this to "guiManger" (?)

    private GameState gameState;
    private List<NodeController> nodes = new List<NodeController>();
    private List<EffectController> effects = new List<EffectController>();
    private List<ConnectionController> connections = new List<ConnectionController>();

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
        rewardManger.OnSelectedReward += InstanceReward;
        timeManager.OnEndCycle += NewDay;
        nodeManager.OnConnect += CreateConnection;
    }

    private void OnApplicationQuit()
    {
        SaveState();
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
        Debug.Log("<color=#70FB5F>[Node Engine, Resources]</color> Load <b>" + nodeDatas.Length + "</b> node resources.");
        effectDatas = Resources.LoadAll<EffectData>("Effects");
        Debug.Log("<color=#70FB5F>[Node Engine, Resources]</color> Load <b>" + effectDatas.Length + "</b> effect resources.");
        ingredientsDatas = Resources.LoadAll<IngredientData>("Materials");
        Debug.Log("<color=#70FB5F>[Node Engine, Resources]</color> Load <b>" + ingredientsDatas.Length + "</b> ingredient resources.");
    }

    private void SaveState()
    {
        // Save nodes
        NodeState[] nodeSave = new NodeState[this.nodes.Count];
        for (int i = 0; i < nodes.Count; i++)
        {
            var name = nodes[i].GetData().name;
            var pos = nodes[i].transform.position;
            var ct = nodes[i].CurrentTime;
            nodeSave[i] = new NodeState(name, pos.x, pos.y, ct);
        }
        gameState.SetNodes(nodeSave);
        Debug.Log("<color=#FFC300>[Node Engine, saveSys]</color> <b>" + nodeSave.Length + "</b> nodes saved.");

        // Save effects
        EffectState[] effectSave = new EffectState[this.effects.Count];
        for (int i = 0; i < effects.Count; i++)
        {
            var name = effects[i].Data.name;
            var ct = effects[i].CurrentTime;
            effectSave[i] = new EffectState(name, ct);
        }
        gameState.SetEffects(effectSave);
        Debug.Log("<color=#FFC300>[Node Engine, saveSys]</color> <b>" + effectSave.Length + "</b> effects saved.");


        // Save Connections
        List<ConnectionState> connectionSave = new List<ConnectionState>();
        /* for (int i = 0; i < nodes.Count; i++)
        {
            int r1 = i,r2 = 0;
            string prod = "";
            var others = nodes[i].GetConnectedOutputNodes(); // length output: 2
            for (int j = 0; j < others.Length; j++)
            {
                r2 = nodes.IndexOf(others[j]);
            }
            connectionSave.Add(new ConnectionState(new Tuple<int, int>(r1, r2), prod)); // fix prod
        }
        */
        gameState.SetConnections(connectionSave.ToArray());
        Debug.Log("<color=#FFC300>[Node Engine, saveSys]</color> <b>" + connectionSave.Count + "</b> connections saved.");

        gameState.Save();
        Debug.Log("<color=#FFC300>[Node Engine, saveSys]</color> GameState saved!");
       
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
        Debug.Log("<color=#FFC300>[Node Engine, saveSys]</color> <b>" + nodes.Count + "</b> nodes loaded.");

        for (int i = 0; i < gameState.GetEffectAmount(); i++)
        {
            var state = gameState.GetEffect(i);
            var time = state.currentTime;
            var data = effectDatas.First(x => x.name == state.effectName);
            CreateEffect(data,time);
        }
        Debug.Log("<color=#FFC300>[Node Engine, saveSys]</color> <b>" + effects.Count + "</b> effects loaded.");

        for (int i = 0; i < gameState.GetConnectionAmount(); i++)
        {
            var state = gameState.GetConnection(i);
            var n1 = state.nodeRelationIndex.Item1;
            var n2 = state.nodeRelationIndex.Item2;
            //var ingr = ingredientsDatas.First(x => x.name == state.ingredientName);
           //CreateConnection(nodes[n1], nodes[n2]);
        }
        Debug.Log("<color=#FFC300>[Node Engine, saveSys]</color> <b>" + connections.Count + "</b> connections loaded.");
    }

    /// <summary>
    /// Intantiate a node in scene with the information delivered by parameters
    /// and stores its reference
    /// </summary>
    /// <param name="nodeData"></param>
    /// <param name="pos"></param>
    /// <param name="startTime"></param>
    private void CreateNode(NodeData nodeData, Vector2 pos, float startTime) 
    {
        var node = Instantiate(node_Pref, pos, Quaternion.identity);
        node.Init(nodeData,startTime);
        nodes.Add(node);
    }

    /// <summary>
    /// Intantiate a effect in scene with the information delivered by parameters
    /// and stores its reference
    /// </summary>
    /// <param name="effectData"></param>
    /// <param name="startTime"></param>
    public void CreateEffect(EffectData effectData,float startTime) 
    {
        var effect = Instantiate(effect_Pref, effectsPivot);
        effect.Init(effectData,startTime);
        effects.Add(effect);
    }

    /// <summary>
    /// Intantiate a connection in scene with the information delivered by parameters
    /// and stores its reference
    /// </summary>
    /// <param name="n1"></param>
    /// <param name="n2"></param>
    public void CreateConnection(NodeController n1, NodeController n2)
    {
        var pos = n1.transform.position;
        var connection = Instantiate(connetion_Pref, pos, Quaternion.identity);
        connection.Connect(n1, n2);
    }

    /// <summary>
    /// Get a spatial position on the canvas that is not being used by any game elements
    /// </summary>
    /// <returns></returns>
    public Vector2 GetUnoccupiedPosition() // IMPLEMENTAR 
    {
        Debug.Log("<color=#FF0000>[Node Engine]</color> Implement this function.");
        return Vector2.zero; 
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
            var rewards = rewardManger.GenerateRewards(nodeDatas, effectDatas, 3, UnityEngine.Random.Range(2, 4), 3, gameState.Day); // nodeDatas y effectDatas podria ser estatica y global en otra clase que guarde datas
            rewardManger.ShowRewards(rewards);
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
#if UNITY_EDITOR
        if(Input.GetKey(KeyCode.P))
        {
            gameState.Money += 1000;
        }
#endif

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

    private void SetMoneyValue(int v) // mover a un controlador GUI
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