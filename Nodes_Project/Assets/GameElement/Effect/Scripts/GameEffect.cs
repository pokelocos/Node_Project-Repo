using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MicroFactory.Effects
{
    [CreateAssetMenu(fileName = "New effect", menuName = "Micro Factory/Effect...")]
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

        [Range(-1, 1)] public float speed = 0; // añade
        [Range(-1, 1)] public float successProbability = 0; // sobreescribe

        protected GameEffect()
        {
        }

        public void ApplyAditiveEffect(List<NodeView> nodes) 
        {
            if (speed != 0)
            {
                foreach (var node in nodes)
                {
                    node.GetNodeData().speed += speed;  // añade
                }
            }

            if (successProbability != 0)
            {
                foreach (var node in nodes)
                {
                    node.GetNodeData().successProbability += successProbability;
                }
            }
        }

        public void RemoveAditiveEffect(List<NodeView> nodes) 
        {
            if (speed != 0)
            {
                foreach (var node in nodes)
                {
                    node.GetNodeData().speed += speed;  // añade
                }
            }

            if (successProbability != 0)
            {
                foreach (var node in nodes)
                {
                    node.GetNodeData().successProbability += successProbability;
                }
            }
        }

        public void ApplyMultiplicativeEffect()
        {

        }

        public void RemoveMultiplicativeEffect()
        {

        }
    }
}
