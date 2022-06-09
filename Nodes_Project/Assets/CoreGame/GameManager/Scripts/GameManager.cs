using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using MicroFactory;
using DataSystem;
using UnityEditor;
using System;

public class GameManager : MonoBehaviour
{

    public int lastpoint = 0;
    public int winPoints = 5;

    public GameObject warningPanel;

    [Space]
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject winPanel;

    [Header("Balance Settings")]
    [SerializeField] private Text mantainance_text;
    [SerializeField] private Text lastEarnings_text;

    private int lastBalance = 0; // balanceManager (?)
    private int lastEarnings; // balanceManager (?)
    private static List<int> dayTransactions = new List<int>(); // balanceManager (?)
    
    // Tool-> talvezs se pueda poner dentro de nodecontroler o lo que deje mover a los nodos(?)
    public static bool snapTool;

    public AudioSource source;
    public AudioClip winSound;

    private static int negativeDays = 0;
    private bool warningonce = true;

    #region ##Events##
    public delegate void NodeEvent(NodeController node);
    public event NodeEvent OnAddNode;
    public event NodeEvent OnRemoveNode;

    public delegate void EffectEvent(EffectController effect);
    public event EffectEvent OnAddEffect;
    public event EffectEvent OnRemoveEffect;

    public delegate void ConnectionEvent(ConnectionController connection);
    public event ConnectionEvent OnAddConection;
    public event ConnectionEvent OnRemoveConection;
    #endregion

    [Header("Managers slaves")]
    [SerializeField] private TimeManager timeManager;
    [SerializeField] private NodeManager nodeManager; 
    [SerializeField] private RewardManager rewardManger;
    [SerializeField] private GuiManager guiManager; //Manager(??)
    //[SerializableField] private AssetLoader assetLoader; (esto tiene que ser estatico (?))

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

    private void Awake()
    {
        LoadAssetsData();
        LoadGameSaved();

        lastBalance = gameState.Money;
        timeManager.SetTimeScale(0);
    }

    private void Start()
    {
        // Set Events
        rewardManger.OnSelectedReward += InstanceReward;
        timeManager.OnEndCycle += NewDay;
        nodeManager.OnConnect += CreateConnection;
    }


    private void OnApplicationQuit()
    {
        SaveState();
    }

    private void Update()
    {
        guiManager.SetContract(gameState.ContractPoints, winPoints); // actualizar en un evento y no en cada update
        guiManager.SetMoneyValue(gameState.Money); // actualizar en un evento y no en cada update

        if (LoseCondition())
        {
            losePanel.SetActive(true);
        }

        if (WinCondition())
        {
            winPanel.SetActive(true);
        }

        if (lastpoint != gameState.ContractPoints)
        {
            source.PlayOneShot(winSound);
        }

        lastpoint = gameState.ContractPoints;
    }

    public bool WinCondition()
    {
        return gameState.ContractPoints >= winPoints;
    }

    public bool LoseCondition()
    {
        return negativeDays >= 3;
    }

    /// <summary>
    /// Load Game state
    /// </summary>
    private void LoadGameSaved()
    {
        var data = DataManager.LoadData<Data>();
        if (data != null)
        {
            this.gameState = data.GameState;
            InitGameState(gameState);
        }
        else
        {
            Debug.LogError("< color =#ff0000>[Load Data Error]</color> Failed to load game information");
        }
    }

    /// <summary>
    /// load the information of the game resources
    /// </summary>
    private void LoadAssetsData() // tabien hay que hacer un "loadModAssets" o seria aqui mismo?
    {
        nodeDatas = Resources.LoadAll<NodeData>("Nodes");
        Debug.Log("<color=#70FB5F>[Node Engine, Resources]</color> Load <b>" + nodeDatas.Length + "</b> node resources.");
        effectDatas = Resources.LoadAll<EffectData>("Effects");
        Debug.Log("<color=#70FB5F>[Node Engine, Resources]</color> Load <b>" + effectDatas.Length + "</b> effect resources.");
        ingredientsDatas = Resources.LoadAll<IngredientData>("Materials");
        Debug.Log("<color=#70FB5F>[Node Engine, Resources]</color> Load <b>" + ingredientsDatas.Length + "</b> ingredient resources.");
    }

    /// <summary>
    /// Save the state of the current game
    /// </summary>
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
        ConnectionState[] connectionSave = new ConnectionState[this.connections.Count];
        for (int i = 0; i < connections.Count; i++)
        {
            int n1 = nodes.IndexOf(connections[i].GetOriginNode());
            int n2 = nodes.IndexOf(connections[i].GetDestinationNode());
            string ingredient = connections[i].GetProduct().data.name;
            connectionSave[i] = new ConnectionState(n1,n2, ingredient);
        }
        gameState.SetConnections(connectionSave);
        Debug.Log("<color=#FFC300>[Node Engine, saveSys]</color> <b>" + connectionSave.Length + "</b> connections saved.");      
    }

    /// <summary>
    /// Initialize and instantiate the elements delivered in the "GameState"
    /// </summary>
    /// <param name="gameState"></param>
    private void InitGameState(GameState gameState)
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
            var ingr = ingredientsDatas.First(x => x.name == state.ingredientName);
            CreateConnection(nodes[n1], nodes[n2]);
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
        connections.Add(connection);
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



    
    public void AddMoney(int amount) // hay que encapsular el manejo de plata o esquematizar mejor sus funciones
    {
        dayTransactions.Add(amount);
        gameState.Money += amount;

        if (gameState.Money > 0) // esto deberia estar en condiciones de ganr perder dentro de gamemode y que se asignen a eventos de esta clase
        {
            negativeDays = 0;
        }
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


        guiManager.SetBalance(balance);
        guiManager.SetDay(gameState.Day);
       

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
    
    public void InstanceReward(Reward reward) // el reward deberia recibor el game manager por parametro y hacer lo que corresponda (o hacrlo con funciones estaticas)
    {
        for (int i = 0; i < reward.nodes.Length; i++)
        {
            CreateNode(reward.nodes[i], GetUnoccupiedPosition(), 0);
        }

        for (int i = 0; i < reward.effects.Length; i++)
        {
            CreateEffect(reward.effects[i], 0);
        }

        AddMoney(-reward.price);
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



}