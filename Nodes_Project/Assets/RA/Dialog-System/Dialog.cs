﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RA.DialogSystem
{
    [CreateAssetMenu(fileName = "New Dialog", menuName = "Dialog System/Dialog...")]
    [System.Serializable]
    public class Dialogue : ScriptableObject
    {
        [SerializeField]
        private List<Sentence> sentences;

        public int Size { get { return sentences.Count; } }

        internal Sentence GetSentence(int i)
        {
            return sentences[i];
        }
    }
}
