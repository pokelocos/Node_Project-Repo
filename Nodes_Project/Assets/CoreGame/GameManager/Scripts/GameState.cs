
using MicroFactory;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState
{
    // basic stadistics
    [SerializeField] private int money;
    [SerializeField] private int day;
    [SerializeField] private int contractPoints; // change name to "points"

    // general data
    [SerializeField] private EffectState[] effects;
    [SerializeField] private NodeState[] nodes;
    [SerializeField] private ConnectionState[] connections;

    [SerializeField] private string gameModeName;
    private string[] activeModsId; // esto no se si deba ir aqui pero es para guadar que mods estaban interviniendo en la partida

    public GameState(int money, int day, int contractPoints, EffectState[] effects, NodeState[] nodes, ConnectionState[] connections,string gameModeName, string[] activeModsId)
    {
        this.money = money;
        this.day = day;
        this.contractPoints = contractPoints;
        this.nodes = nodes;
        this.effects = effects;
        this.connections = connections;
        this.gameModeName = gameModeName;
        this.activeModsId = activeModsId;
    }

    public GameState()
    {
        this.money = 0;
        this.day = 0;
        this.contractPoints = 0;
        this.nodes = new NodeState[0];
        this.effects = new EffectState[0];
        this.connections = new ConnectionState[0];
        this.gameModeName = "No Mode";
        this.activeModsId = new string[0];
    }

    /// <summary>
    /// Actualiza el archivo de guardado con las variables entregadas sobreescribiendo la infromacion anterior.
    /// </summary>
    /// <param name="money"></param>
    /// <param name="day"></param>
    /// <param name="contractPoints"></param>
    /// <param name="effects"></param>
    /// <param name="nodes"></param>
    public void Save(int money, int day, int contractPoints, EffectState[] effects, NodeState[] nodes, ConnectionState[] connections) //!!!
    {
        this.money = money;
        this.day = day;
        this.contractPoints = contractPoints;
        this.effects = effects;
        this.nodes = nodes;
        this.connections = connections;

        this.gameModeName = "No Mode";

        var data = DataSystem.StaticData.Data;
        data.GameState = this;
        DataSystem.StaticData.Data = data;
    }

    /// <summary>
    /// Actualiza el archivo de guardado con las variables de este objeto sin sobre escribir nada.
    /// </summary>
    public void Save()
    {
        var data = DataSystem.StaticData.Data;
        data.GameState = this;
        DataSystem.StaticData.Data = data;
    }

    public int Money { get { return money; } set { money = value; } }
    public int Day { get { return day; } set { day = value; } }
    public int ContractPoints { get { return contractPoints; } set { contractPoints = value; } }

    #region ##Set and Get Effects##

    public void SetEffects(EffectState[] effects)
    {
        this.effects = effects;
    }

    public EffectState GetEffect(int i)
    {
        return effects[i];
    }

    public int GetEffectAmount()
    {
        return effects.Length;
    }

    #endregion

    #region ##Set and Get Nodes##

    public void SetNodes(NodeState[] nodes)
    {
        this.nodes = nodes;
    }

    public NodeState GetNode(int i)
    {
        return nodes[i];
    }

    public int GetNodeAmount()
    {
        return nodes.Length;
    }

    #endregion

    #region ##Set and Get Connections##

    public void SetConnections(ConnectionState[] connections)
    {
        this.connections = connections;
    }

    public ConnectionState GetConnection(int i)
    {
        return connections[i];
    }

    public int GetConnectionAmount()
    {
        return connections.Length;
    }

    #endregion

}