using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MicroFactory
{
    [CreateAssetMenu(fileName = "New effect", menuName = "Micro Factory/Effect...")]
    public class EffectData : ScriptableObject
    {
        [Header("Basic info")]
        public Sprite icon = null;
        public string title = "";
        public string description = "";
        public Color color = Color.gray;
        public float duration = 180;

        [Header("Affected nodes")]
        public NodeData.Type[] byTypes;
        public string[] byNames;

        [Range(-1, 1)] public float speed = 0; // añade
        [Range(-1, 1)] public float successProbability = 0; // sobreescribe

        protected EffectData()
        {
        }

        public void ApplyAditiveEffect(List<NodeController> nodes)  // esto deberia estar aqui????, supongo que si
        {
            if (speed != 0)
            {
                foreach (var node in nodes)
                {
                    node.GetData().speed += speed;  // añade
                }
            }

            if (successProbability != 0)
            {
                foreach (var node in nodes)
                {
                    node.GetData().successProbability += successProbability;
                }
            }
        }

        public void RemoveAditiveEffect(List<NodeController> nodes) // esto deberia estar aqui????, supongo que si
        {
            if (speed != 0)
            {
                foreach (var node in nodes)
                {
                    node.GetData().speed += speed;  // añade
                }
            }

            if (successProbability != 0)
            {
                foreach (var node in nodes)
                {
                    node.GetData().successProbability += successProbability;
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
