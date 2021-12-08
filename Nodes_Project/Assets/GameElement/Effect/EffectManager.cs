using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MicroFactory.Effects
{
    class EffectManager : MonoBehaviour
    {
        public Transform transformParent;
        public EffectView pref_view;

        //private List<EffectView> views = new List<EffectView>();


        public void AddEffects(GameEffect[] effects)
        {
            foreach (var effect in effects)
                AddEffect(effect);
        }

        public void AddEffect(GameEffect effect)
        {
            var v = Instantiate(pref_view, transformParent);
            v.SetInfo(effect, effect.color, Color.white);

            var nodes = new List<NodeView>();
            nodes.Concat(GetNodesByTypes(effect.byTypes));
            nodesConcat(GetNodesByNames(effect.byNames).Where(x => !nodes.Contains(x)));
        }



        public List<NodeView> GetNodesByTypes(NodeData.Type[] types)
        {
            List<NodeView> toReturn = new List<NodeView>();
            NodeView[] allNodes = FindObjectsOfType<NodeView>();

            foreach (var ty in types)
            {
                var nodes = allNodes.Where(x => x.GetNodeData().type == ty && !toReturn.Contains(x));
                toReturn.Concat(nodes);
            }

            return toReturn;
        }

        public List<NodeView> GetNodesByNames(string[] names)
        {
            List<NodeView> toReturn = new List<NodeView>();
            NodeView[] allNodes = FindObjectsOfType<NodeView>();

            foreach (var name in names)
            {
                var nodes = allNodes.Where(x => x.GetNodeData().name == name && !toReturn.Contains(x));
                toReturn.Concat(nodes);
            }

            return toReturn;
        }
    }
}
