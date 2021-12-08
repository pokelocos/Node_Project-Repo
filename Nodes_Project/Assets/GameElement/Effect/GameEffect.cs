using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MicroFactory.Effects
{
    public class GameEffect : ScriptableObject
    {
        [Header("Basic info")]
        public Sprite icon = null;
        public string title = "";
        public string description = "";
        public Color color = Color.gray;
        public float duration = 60*3;

        [Header("Affected nodes")]
        public NodeData.Type[] byTypes;
        public string[] byNames;

        [Header("Speed")]
        [Range(-1, 1)] public float speed = 0; // añade
        [Header("Success probability")]
        [Range(-1, 1)] public float successProbability = 0; // sobreescribe

        protected GameEffect()
        {
        }

        public void ApplyEffect(List<NodeView> nodes) // probablemente esto lo mueva al controlador
        {
            if (speed != 0)
            {
                foreach (var node in nodes)
                {
                    node.GetNodeData().speed = speed; // esto deveria ser aditivo y no remplazar
                }
            }

            if (successProbability != 0)
            {
                foreach (var node in nodes)
                {
                    node.GetNodeData().successProbability = successProbability;
                }
            }
        }
    }
}
