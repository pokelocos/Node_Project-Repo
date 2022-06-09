using System;
using System.Collections.Generic;
using MicroFactory;
using UnityEngine;

namespace MicroFactory
{
    [CreateAssetMenu(fileName = "New Game Mode", menuName = "Micro Factory/GameMode...")]
    public class GameMode : ScriptableObject
    {
        public string modeName;
        public string description;

        public int money = 1000;
        public int maxPoint = 5;
        [SerializeField] private List<_NodesGameMode> nodes;
        [SerializeField] private List<EffectData> effects;
        [SerializeField] private List<_ConnectionGameMode> conections;

        public bool allowMods = false; // ??
        public bool incrementalDificulty = false; // dificty change relative a player lvl

        public NodeData GetNode(int i)
        {
            return nodes[i].data;
        }

        public Vector2 GetNodePosition(int i)
        {
            return nodes[i].position;
        }

        public int GetNodeAmount()
        {
            return this.nodes.Count;
        }

        public EffectData GetEffect(int i)
        {
            return effects[i];
        }

        public int GetTotalEffectsAmount()
        {
            return this.effects.Count;
        }

        public IngredientData GetConnectionIngredient(int i)
        {
            return conections[i].ingredient;
        }

        public Vector2Int GetConnectionRelation(int i)
        {
            return conections[i].relation;
        }

        public int GetConnectionAmount()
        {
            return this.conections.Count;
        }

        [Serializable]
        private class _NodesGameMode
        {
            [SerializeField] public NodeData data;
            [SerializeField] public Vector2 position;
        }

        [Serializable]
        private class _ConnectionGameMode
        {
            [SerializeField] public IngredientData ingredient;
            [SerializeField] public Vector2Int relation;
        }
    }
}