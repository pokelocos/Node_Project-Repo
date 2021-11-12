using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RA.DialogSystem
{
    [CreateAssetMenu(fileName = "New Actor", menuName = "RA/Dialog System... /New actor")]
    public class Actor : ScriptableObject
    {
        [Tooltip("-1 or default Image")]
        public Sprite defaultImg;

        [Tooltip("1 = happy, 2 = angry, 3 = sad, 4 = sourpriced")]
        public Sprite[] emotions = new Sprite[4];
        public string actorName;

        public Sprite GetSprite(int i)
        {
            if (i == -1)
                return defaultImg;
            return emotions[i];
        }
    }
}
