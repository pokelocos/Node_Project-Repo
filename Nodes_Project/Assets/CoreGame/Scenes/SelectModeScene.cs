using DataSystem;
using MicroFactory;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectModeScene : MonoBehaviour
{
    private GameMode modeSelected;
    
    [Header("Scene references")]
    [SerializeField] private Text descriptionText;
    [SerializeField] private Button playButton;

    private void Awake()
    {
        playButton.interactable = modeSelected != null;
        //playButton.onClick.AddListener(()=>{ InitGameRun(modeSelected); });
    }

    public void SelectMode(GameMode gameMode) // button function
    {
        modeSelected = gameMode;
        descriptionText.text = gameMode.description;
        playButton.interactable = true;
    }

    // estas funcion deberia ser algo tipo "utils" de "Core Game" o no se

    public void InitGameRun(GameMode mode) //"GameMode.data" to "GameState"
    {
        Data data = StaticData.Data;
        var gameState = new GameState();

        gameState.Money = mode.money;

        // "gamemode.node" to "nodestate"
        var nodes = new NodeState[mode.GetNodeAmount()];
        for (int i = 0; i < mode.GetNodeAmount(); i++)
        {
            var node = mode.GetNode(i);
            var pos = mode.GetNodePosition(i);
            nodes[i] = new NodeState(node.name,pos.x,pos.y,0);
        }
        gameState.SetNodes(nodes);

        // "gamemode.effects" to "effectstate"
        var effects = new EffectState[mode.GetTotalEffectsAmount()];
        for (int i = 0; i < mode.GetTotalEffectsAmount(); i++)
        {
            var effect = mode.GetEffect(i);
            effects[i] = new EffectState(effect.name,0);
        }
        gameState.SetEffects(effects);

        // "gameMode.connections" to "connectionstate"
        var conects = new ConnectionState[mode.GetConnectionAmount()];
        for (int i = 0; i < mode.GetConnectionAmount(); i++)
        {
            var ingredient = mode.GetConnectionIngredient(i);
            var relation = mode.GetConnectionRelation(i);
            conects[i] = new ConnectionState(new Tuple<int, int>(relation.x, relation.y),ingredient.name);
        }
        gameState.SetConnections(conects);

        //gamestate.setActiveMods(new string[0]); // not implemented

        data.GameState = gameState;
        DataManager.SaveData<Data>(data);
    }

    public void InitGameRun()
    {
        InitGameRun(modeSelected);
    }
}
